using System;
using System.Collections.Generic;
using MonsterTradingCardGame.Classes;
using MonsterTradingCardGame.Enum;
using Npgsql;
using NpgsqlTypes;
using BCryptNet = BCrypt.Net.BCrypt;

namespace MonsterTradingCardGame.PostgreDB
{
    class DBConn
    {
        private string _connString = "Server=localhost;Port=5432;User Id=postgres;Password=admin;Include Error Detail = true";

        private static NpgsqlConnection Connection(string connString)
        {
            var conn = new NpgsqlConnection(connString);
            conn.Open();
            return conn;
        }

        public User LoginUser(string username, string pwd)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select \"userID\", \"userName\", \"userCoins\", \"userElo\", \"userPwd\", \"userWins\", \"userLoses\" from \"gameUser\" where \"userName\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);
                
                dr = userCmd.ExecuteReader();
                User user = null;
                while (dr.Read())
                {
                    user = new User(dr.GetInt32(0),dr.GetString(1), dr.GetInt32(2), dr.GetInt32(3), dr.GetString(4), dr.GetInt32(5), dr.GetInt32(6));
                }

                if (user != null && BCryptNet.Verify(pwd, user.password))
                {
                    //Console.WriteLine();
                    //user.PrintUserInformation();
                    //Console.ReadLine();
                    dr.Close();
                    return user;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public User RegisterUser(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "insert into \"gameUser\" (\"userName\" , \"userElo\", \"userCoins\", \"userPwd\") values(@p1, @p2, @p3, @p4) returning \"userID\";",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.UniqueUsername);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserElo);
                userCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, user.Coins);
                userCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Varchar, user.password);
                userCmd.ExecuteNonQuery();
                transaction.Commit();

                int userID = GetUserIDReg(user.UniqueUsername);

                if (userID != 0)
                {
                    //Console.WriteLine($"DB register; good");
                    User registerUser = new User(userID, user.UniqueUsername, user.Coins, user.UserElo, user.password, user.Wins, user.Loses);
                    //Console.WriteLine($"user id: {registerUser.UserID}");
                    //Console.ReadLine();
                    return registerUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public int GetUserIDReg(string username)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            int userID = 0;
            try
            {
                using var userIDCmd = new NpgsqlCommand(
                    "select \"userID\" from \"gameUser\" where \"userName\" = @p1;",
                    conn);
                userIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, username);
                dr = userIDCmd.ExecuteReader();

                while (dr.Read())
                {
                    userID = dr.GetInt32(0);
                    //Console.WriteLine($"\n{userID}");
                     
                    Console.ReadLine();
                }

                dr.Close();
                return userID;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return userID;
            }

        }

        public List<Cards> GetUserPlayCardStack(User user)
        {
            List<int> cardIDs = new List<int>();
            List<Cards> cards = new List<Cards>();

            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"cardID\" from \"userPlayCards\" where \"isInDeck\" = true and \"gameUserID\" = @p1;",
                    conn);

                cardIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = cardIDCmd.ExecuteReader();
                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();

                foreach (var i in cardIDs)
                {
                    using var cardCmd = new NpgsqlCommand(
                        "select * from \"allCards\" where \"cardID\" = @p1;",
                        conn);

                    cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, i);

                    dr = cardCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cards card = new Cards(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), (CardTypesEnum.CardElementEnum)dr.GetInt32(3), (CardTypesEnum.CardTypeEnum)dr.GetInt32(4));
                        cards.Add(card);
                    }
                    dr.Close();
                }
                
                return cards.Count > 0 ? cards : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }
        public List<Cards> GetUserAllCardStack(User user)
        {
            List<int> cardIDs = new List<int>();
            List<Cards> cards = new List<Cards>();

            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"cardID\" from \"userPlayCards\" where \"isInTrade\" = false and \"gameUserID\" = @p1;",
                    conn);

                cardIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = cardIDCmd.ExecuteReader();
                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();

                foreach (var i in cardIDs)
                {
                    using var cardCmd = new NpgsqlCommand(
                        "select * from \"allCards\" where \"cardID\" = @p1;",
                        conn);

                    cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, i);

                    dr = cardCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        Cards card = new Cards(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), (CardTypesEnum.CardElementEnum)dr.GetInt32(3), (CardTypesEnum.CardTypeEnum)dr.GetInt32(4));
                        cards.Add(card);
                    }
                    dr.Close();
                }

                return cards.Count > 0 ? cards : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }
        
        public bool ChangePlayDeck(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var downCmd = new NpgsqlCommand(
                    "update \"userPlayCards\" set \"isInDeck\" = false where \"gameUserID\" = @p1;",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);
                downCmd.ExecuteNonQuery();
                transaction.Commit();

                foreach (var i in user.UserPlayCardStack)
                {
                    using var updateCmd = new NpgsqlCommand(
                        "update \"userPlayCards\" set \"isInDeck\" = true where \"gameUserID\" = @p1 and \"cardID\" = @p2;",
                        conn);

                    updateCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);
                    updateCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, i.CardID);
                    updateCmd.ExecuteNonQuery();
                    //transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public bool AddUserPlayCards(User user, List<int> cardIDs)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                foreach (var id in cardIDs)
                {
                    using var userCmd = new NpgsqlCommand(
                        "insert into \"userPlayCards\" (\"gameUserID\", \"cardID\") VALUES (@p1,@p2);",
                        conn);

                    userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);
                    userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, id);
                    userCmd.ExecuteNonQuery();
                    transaction.Commit();
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                return false;
            }
        }

        public User UpdatedUser(User user)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select \"userID\", \"userName\", \"userCoins\", \"userElo\", \"userPwd\", \"userWins\", \"userLoses\" from \"gameUser\" where \"userID\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = userCmd.ExecuteReader();
                User updUser = null;
                while (dr.Read())
                {
                    updUser = new User(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), dr.GetInt32(3), dr.GetString(4), dr.GetInt32(5), dr.GetInt32(6));
                }

                if (updUser != null && user.password == updUser.password)
                {
                    //Console.WriteLine();
                    //user.PrintUserInformation();
                    //Console.ReadLine();
                    dr.Close();
                    return updUser;
                }

                return user;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public bool UpdateWin(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            const int eloGain = 3;
            const int win = 1;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "update \"gameUser\" set \"userElo\" = @p1, \"userWins\" = @p2 where \"userID\" = @p3;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserElo + eloGain);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.Wins + win);
                userCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, user.UserID);
                userCmd.ExecuteNonQuery();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public bool UpdateLos(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            const int eloLos = 5;
            const int los = 1;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "update \"gameUser\" set \"userElo\" = @p1, \"userLoses\" = @p2 where \"userID\" = @p3;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserElo - eloLos);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.Loses + los);
                userCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, user.UserID);
                userCmd.ExecuteNonQuery();
                transaction.Commit();

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public User GetBattleEnemy(User user)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            List<int> users = new List<int>();
            int enemyID = -1;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select distinct \"gameUserID\" from \"userPlayCards\" where \"gameUserID\" != @p1; ",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = userCmd.ExecuteReader();
                while (dr.Read())
                {
                    users.Add(dr.GetInt32(0));
                }
                dr.Close();

                foreach (var id in users)
                {
                    using var idCmd = new NpgsqlCommand(
                        "select sum(\"isInDeck\"::int) from \"userPlayCards\" where \"gameUserID\" = @p1; ; ",
                        conn);

                    idCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, id);

                    dr = idCmd.ExecuteReader();
                    while (dr.Read())
                    {
                        //Console.WriteLine($"{dr.GetInt32(0)} is sum of {}");
                        if (dr.GetInt32(0) != 4) continue;
                        enemyID = id;
                        break;
                    }
                    dr.Close();

                    if (enemyID > -1)
                        break;
                }

                User enemyUser = GetUserByID(enemyID);
                return enemyUser;

            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }

        public User GetUserByID(int userID)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select \"userID\", \"userName\", \"userCoins\", \"userElo\", \"userPwd\", \"userWins\", \"userLoses\" from \"gameUser\" where \"userID\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, userID);

                dr = userCmd.ExecuteReader();
                User updUser = null;
                while (dr.Read())
                {
                    updUser = new User(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), dr.GetInt32(3), dr.GetString(4), dr.GetInt32(5), dr.GetInt32(6));
                }

                if (updUser != null)
                {
                    //Console.WriteLine("GetUserByID");
                    //updUser.PrintUserInformation();
                    //Console.ReadLine();
                    dr.Close();
                    return updUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                return null;
            }
        }
    }
}

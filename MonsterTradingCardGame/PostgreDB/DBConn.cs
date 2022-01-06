using System;
using System.Collections.Generic;
using System.Linq;
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
                Console.ReadLine();
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
                    User registerUser = new User(userID, user.UniqueUsername, user.Coins, user.UserElo, user.password, user.Wins, user.Loses);
                    return registerUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
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
                Console.ReadLine();
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
                Console.ReadLine();
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
                Console.ReadLine();
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
                        "update \"userPlayCards\" set \"isInDeck\" = true where \"gameUserID\" = 36 and \"playCardsID\" = (select \"playCardsID\" from \"userPlayCards\" where \"gameUserID\" = @p1 and \"cardID\" = @p2 limit 1);",
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
                }
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
                    dr.Close();
                    return updUser;
                }

                return user;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return user;
            }
        }

        public bool UpdateUserData(User user)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "update \"gameUser\" set \"userName\" = @p1, \"userPwd\" = @p2 where \"userID\" = @p3;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Varchar, user.UniqueUsername);
                userCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Varchar, user.password);
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

                while (users.Count > 0)
                {
                    Random rand = new Random();
                    int randInt = rand.Next(0, users.Count);
                    int id = users.ElementAt(randInt);
                    users.RemoveAt(randInt);

                    if (id > 0)
                    {
                        using var idCmd = new NpgsqlCommand(
                            "select sum(\"isInDeck\"::int) from \"userPlayCards\" where \"gameUserID\" = @p1;",
                            conn);

                        idCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, id);

                        dr = idCmd.ExecuteReader();

                        while (dr.Read())
                        {
                            if (dr.GetInt32(0) != 4) continue;
                            enemyID = id;
                            break;
                        }
                        dr.Close();

                        if (enemyID > 0)
                            break;
                    }
                }

                User enemyUser = GetUserByID(enemyID);
                return enemyUser;

            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
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
                    dr.Close();
                    return updUser;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }

        public List<(string, int)> GetScoreByElo()
        {
            var tuplelist = new List<(string, int)>();
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var eloCmd = new NpgsqlCommand(
                    "select \"userName\", \"userElo\" from \"gameUser\" order by\"userElo\" desc;",
                    conn);

                dr = eloCmd.ExecuteReader();

                while (dr.Read())
                {
                    tuplelist.Add((dr.GetString(0), dr.GetInt32(1)));
                }

                dr.Close();

                return tuplelist.Count > 0 ? tuplelist : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }
        public List<(string, int)> GetScoreByGames()
        {
            var tuplelist = new List<(string, int)>();
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var eloCmd = new NpgsqlCommand("select \"userName\", case when \"userLoses\" = 0 and \"userWins\" = 0 then 0 when \"userLoses\" = 0 and \"userWins\" > 0 then 100 when \"userLoses\" > 0 and \"userWins\" > 0 or \"userWins\" = 0 then \"userWins\" / (\"userWins\" + \"userLoses\") * 100 end from \"gameUser\" order by \"userWins\" desc; ", conn);

                dr = eloCmd.ExecuteReader();

                while (dr.Read())
                {
                    tuplelist.Add((dr.GetString(0), dr.GetInt32(1)));
                }

                dr.Close();

                return tuplelist.Count > 0 ? tuplelist : null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }

        public bool GetMonsterTypePackages(User user, int type)
        {
            int monsterType = 0;
            using var conn = Connection(_connString);
            NpgsqlDataReader dr = null;
            try
            {
                List<int> cardIDs = new List<int>();
                using var cardCmd = new NpgsqlCommand("select \"cardID\" from \"allCards\" where \"cardElement\" = @p1 and \"cardType\" = @p2 ;", conn);

                cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, type);
                cardCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, monsterType);

                dr = cardCmd.ExecuteReader();

                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();
                List<int> randIDs = new List<int>();
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int randInt = rand.Next(0, cardIDs.Count);
                    int id = cardIDs.ElementAt(randInt);
                    cardIDs.RemoveAt(randInt);
                    
                    randIDs.Add(id);
                }

                if (AddUserPlayCards(user, randIDs))
                {
                    return true;
                }
                return false;


            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public bool GetRandPackages(User user)
        {
            int monsterType = 0;
            using var conn = Connection(_connString);
            NpgsqlDataReader dr = null;
            try
            {
                List<int> cardIDs = new List<int>();
                using var cardCmd = new NpgsqlCommand("select \"cardID\" from \"allCards\" where \"cardType\" = @p1 ;", conn);

                cardCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, monsterType);

                dr = cardCmd.ExecuteReader();

                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();
                List<int> randIDs = new List<int>();
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int randInt = rand.Next(0, cardIDs.Count);
                    int id = cardIDs.ElementAt(randInt);
                    cardIDs.RemoveAt(randInt);

                    randIDs.Add(id);
                }

                if (AddUserPlayCards(user, randIDs))
                {
                    return true;
                }
                return false;


            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public bool GetMixedPackages(User user)
        {
            using var conn = Connection(_connString);
            NpgsqlDataReader dr = null;
            try
            {
                List<int> cardIDs = new List<int>();
                using var cardCmd = new NpgsqlCommand("select \"cardID\" from \"allCards\" ;", conn);

                dr = cardCmd.ExecuteReader();

                while (dr.Read())
                {
                    cardIDs.Add(dr.GetInt32(0));
                }

                dr.Close();
                List<int> randIDs = new List<int>();
                Random rand = new Random();
                for (int i = 0; i < 5; i++)
                {
                    int randInt = rand.Next(0, cardIDs.Count);
                    int id = cardIDs.ElementAt(randInt);
                    cardIDs.RemoveAt(randInt);

                    randIDs.Add(id);
                }

                if (AddUserPlayCards(user, randIDs))
                {
                    return true;
                }
                return false;


            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return false;
            }
        }

        public int CheckCoins(User user)
        {
            int coins = 0;
            
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var coinCmd = new NpgsqlCommand(
                    "select \"userCoins\" from \"gameUser\" where \"userID\" = @p1;",
                    conn);
                coinCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = coinCmd.ExecuteReader();

                while (dr.Read())
                {
                    coins = dr.GetInt32(0);
                }

                dr.Close();

                return coins;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                coins = 0;
                return coins;
            }
        }
        public bool ReduceUserCoins(User user, int cost)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                if (CheckCoins(user) < cost)
                {
                    return false;
                }
                using var downCmd = new NpgsqlCommand(
                    "update \"gameUser\" set \"userCoins\" = @p1 where \"userID\" = @p2;",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, CheckCoins(user) - cost);
                downCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserID); 
                
                downCmd.ExecuteNonQuery();
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

        public bool IncreaseUserCoins(User user, int cost)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var downCmd = new NpgsqlCommand(
                    "update \"gameUser\" set \"userCoins\" = @p1 where \"userID\" = @p2;",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, CheckCoins(user) + cost);
                downCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserID);

                downCmd.ExecuteNonQuery();
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

        public List<Cards> TradeAbleCards(User user)
        {
            List<int> cardIDs = new List<int>();
            List<Cards> cards = new List<Cards>();

            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"cardID\" from \"userPlayCards\" where \"isInDeck\" = false and \"isInTrade\" = false and \"gameUserID\" = @p1;",
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

                return cards;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }

        public bool CreateCardTrade(User user, int chosenCardID, int tradeType, int toTradeAmt)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var tradeCmd = new NpgsqlCommand(
                    "insert into \"cardTrades\" (\"tradeUserID\" , \"tradeCardID\", \"tradeType\", \"toTradeAmt\") values(@p1, @p2, @p3, @p4);", conn);

                tradeCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);
                tradeCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, chosenCardID);
                tradeCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, tradeType);
                tradeCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Bigint, toTradeAmt);
                tradeCmd.ExecuteNonQuery();
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

        public bool UpdateTrade(User user, int cardID, bool inTrade)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var downCmd = new NpgsqlCommand(
                    "update \"userPlayCards\" set \"isInTrade\" = @p1 where \"playCardsID\" = (select \"playCardsID\" from \"userPlayCards\" where \"gameUserID\" = @p2 and \"cardID\" = @p3 and \"isInDeck\" = false and \"isInTrade\" = false limit 1);",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Boolean, inTrade);
                downCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserID);
                downCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, cardID);

                downCmd.ExecuteNonQuery();
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
        
        public List<TradeHelper> GetAllTrades(User user)
        {
            List<int[]> cardTrades = new List<int[]>();
            List<TradeHelper> offersTradeHelpers = new List<TradeHelper>();

            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var cardIDCmd = new NpgsqlCommand(
                    "select \"tradeUserID\", \"tradeCardID\",\"tradeType\",\"toTradeAmt\" from \"cardTrades\" where \"tradeUserID\" != @p1;",
                    conn);

                cardIDCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, user.UserID);

                dr = cardIDCmd.ExecuteReader();
                while (dr.Read())
                {
                    TradeHelper helper = new TradeHelper(GetUserByID(dr.GetInt32(0)), GetCardByID(dr.GetInt32(1)), (CardTypesEnum.TradeTypeEnum)dr.GetInt32(2), dr.GetInt32(3));
                    offersTradeHelpers.Add(helper);
                }
                dr.Close();

                return offersTradeHelpers;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }
        public Cards GetCardByID(int cardID)
        {
            using var conn = Connection(_connString);
            //var transaction = conn.BeginTransaction();
            NpgsqlDataReader dr = null;
            try
            {
                using var userCmd = new NpgsqlCommand(
                    "select \"cardID\", \"cardName\", \"cardDmg\", \"cardElement\", \"cardType\" from \"allCards\" where \"cardID\" = @p1;",
                    conn);

                userCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, cardID);

                dr = userCmd.ExecuteReader();
                Cards card = null;
                while (dr.Read())
                {
                    card = new Cards(dr.GetInt32(0), dr.GetString(1), dr.GetInt32(2), (CardTypesEnum.CardElementEnum)dr.GetInt32(3), (CardTypesEnum.CardTypeEnum)dr.GetInt32(4));
                }

                if (card != null)
                {
                    return card;
                }

                return null;
            }
            catch (Exception ex)
            {
                dr?.Close();
                Console.WriteLine($"catch Exception: {ex}");
                Console.ReadLine();
                return null;
            }
        }

        public bool ConfirmedTrade(User user, int oldUser, int cardID)
        {
            const bool inTrade = false;
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var downCmd = new NpgsqlCommand(
                    "update \"userPlayCards\" set \"isInTrade\" = @p1, \"gameUserID\" = @p2 where \"playCardsID\" = (select \"playCardsID\" from \"userPlayCards\" where \"gameUserID\" = @p3 and \"cardID\" = @p4 and \"isInDeck\" = false and \"isInTrade\" = true limit 1);",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Boolean, inTrade);
                downCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, user.UserID);
                downCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, oldUser);
                downCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Bigint, cardID);

                downCmd.ExecuteNonQuery();
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

        public bool DeleteTradeOffer(TradeHelper helper)
        {
            using var conn = Connection(_connString);
            var transaction = conn.BeginTransaction();
            try
            {
                using var downCmd = new NpgsqlCommand(
                    "delete from \"cardTrades\" where \"tradeID\" = (select \"tradeID\" from \"cardTrades\" where \"tradeUserID\" = @p1 and \"tradeCardID\" = @p2 and \"tradeType\" = @p3 and \"toTradeAmt\" = @p4 limit 1);",
                    conn);

                downCmd.Parameters.AddWithValue("p1", NpgsqlDbType.Bigint, helper.TradeUser.UserID);
                downCmd.Parameters.AddWithValue("p2", NpgsqlDbType.Bigint, helper.TradeCard.CardID);
                downCmd.Parameters.AddWithValue("p3", NpgsqlDbType.Bigint, (int)helper.TradeType);
                downCmd.Parameters.AddWithValue("p4", NpgsqlDbType.Bigint, helper.TradeAmnt);

                downCmd.ExecuteNonQuery();
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
    }
}

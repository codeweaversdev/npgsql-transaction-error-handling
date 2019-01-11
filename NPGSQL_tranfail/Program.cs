using System;
using Npgsql;

namespace NPGSQL_tranfail
{
    class Program
    {
        static void Main(string[] args)
        {
            var connectionstring = args[0];
            var query = " SELECT pg_sleep(20000); ";
            try
            {
                using(var connection = new NpgsqlConnection(connectionstring))
                {
                    connection.Open();
                    using(var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            using(var command = new NpgsqlCommand(query, connection, transaction))
                            {
                                using(var reader = command.ExecuteReader())
                                {
                                    var recordsChanged = reader.RecordsAffected;
                                }
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("----------------------------");
                            Console.WriteLine(e);
                            Console.WriteLine("----------------------------");
                        }
                        finally
                        {
                            Console.WriteLine("In the finally!");
                            transaction.Rollback();
                        }
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("============================");
                Console.WriteLine(e);
                Console.WriteLine("============================");
            }
			// at this point, if you run SELECT * FROM pg_stat_activity WHERE state != 'idle'; on pg, the command is still active.
            Console.WriteLine("Press the any key ....");
            Console.ReadLine();
        }
    }
}

// Even after the application has ended... if you run SELECT * FROM pg_stat_activity WHERE state != 'idle'; on pg, the command is still active.
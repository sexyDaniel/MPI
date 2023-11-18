using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using Dapper;
using MPI_2.DTOs;
using MPI_2.Queries;

namespace MPI_2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            if (args[0] == "insert")
                Insert(args);
            else
                Select(args);

        }

        static void Insert(string[] args)
        {
            MPI.Environment.Run(ref args, comm =>
            {
                if (comm.Rank == 0)
                {
                    var timer = new System.Diagnostics.Stopwatch();
                    timer.Start();

                    if (comm.Size == 1)
                    {
                        for (int i = 0; i * 1000 < Queries.Queries.Inserts.Count; i++)
                        {
                            var query = string.Join(",", Queries.Queries.Inserts.Skip(i * 1000).Take(1000));
                            DbContext.AddProducts(query);
                        }
                    }
                    else
                    {
                        for (int i = 1; i < comm.Size; i++)
                        {
                            comm.Receive<int>(i, 0);
                        }
                    }
                    timer.Stop();
                    Console.WriteLine($"time - {timer.ElapsedMilliseconds}");
                }
                else
                {
                    var timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    var total = Queries.Queries.Inserts.Count / (comm.Size-1);
                    var skip = total * (comm.Rank - 1);
                    for (int i = 0; i * 1000 < total; i++)
                    {
                        var take = Math.Min(total - i * 1000, 1000);
                        var query = string.Join(",", Queries.Queries.Inserts.Skip(skip + i*1000).Take(take));
                        DbContext.AddProducts(query);
                    }
                    timer.Stop();
                    Console.WriteLine($"time - {timer.ElapsedMilliseconds}");
                    comm.Send(comm.Rank, 0, 0);
                }
            });
        }

        static void Select(string[] args)
        {
            MPI.Environment.Run(ref args, comm =>
            {
                if (comm.Rank == 0)
                {
                    var timer = new System.Diagnostics.Stopwatch();
                    timer.Start();
                    List<Product> total = new List<Product>();

                    if (comm.Size == 1)
                    {
                        var query = string.Join(" union all ", Queries.Queries.Unions);
                        total = DbContext.GetProducts(query);
                    }
                    else
                    {
                        for (int i = 1; i < comm.Size; i++)
                        {
                            var temp = comm.Receive<List<Product>>(i, 0);
                            total = total.Concat(temp).ToList();
                        }
                    }

                    timer.Stop();
                    Console.WriteLine($"time - {timer.ElapsedMilliseconds} count - {total.Count}");
                }
                else
                {
                    var total = Queries.Queries.Unions.Count / (comm.Size - 1);
                    var skip = total * (comm.Rank - 1);
                    var res = DbContext.GetProducts(string.Join(" union all ", Queries.Queries.Unions.Skip(skip).Take(total)));
                    comm.Send(res, 0, 0);
                }
            });
        }
    }
}

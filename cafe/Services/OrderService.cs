using System.Collections.Generic;
using System.Threading.Tasks;
using Npgsql;
using RestaurantApp.Models;

namespace RestaurantApp.Services
{
    public class OrderService
    {
        private readonly string _connectionString;

        public OrderService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<List<Order>> GetAllOrdersAsync()
        {
            var orders = new List<Order>();
            using (var conn = new NpgsqlConnection(_connectionString))
            {
                await conn.OpenAsync();
                using (var cmd = new NpgsqlCommand("SELECT * FROM orders ORDER BY created_at DESC;", conn))
                {
                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            orders.Add(new Order
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("id")),
                                TableId = reader.IsDBNull(reader.GetOrdinal("table_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("table_id")),
                                WaiterId = reader.IsDBNull(reader.GetOrdinal("waiter_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("waiter_id")),
                                ShiftId = reader.IsDBNull(reader.GetOrdinal("shift_id")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("shift_id")),
                                OrderStatus = reader.GetString(reader.GetOrdinal("order_status")),
                                PaymentMethod = reader.GetString(reader.GetOrdinal("payment_method")),
                                PaymentStatus = reader.GetString(reader.GetOrdinal("payment_status")),
                                TotalAmount = reader.GetDecimal(reader.GetOrdinal("total_amount")),
                                CreatedAt = reader.GetDateTime(reader.GetOrdinal("created_at")),
                                UpdatedAt = reader.GetDateTime(reader.GetOrdinal("updated_at")),
                                CompletedAt = reader.IsDBNull(reader.GetOrdinal("completed_at")) ? null : (DateTime?)reader.GetDateTime(reader.GetOrdinal("completed_at"))
                            });
                        }
                    }
                }
            }

            return orders;
        }
        
    }
}
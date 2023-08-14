using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Data.SqlClient;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using WebApplication1.Models;

namespace WebApplication1.Controllers.DataAccess
{
    public class DataAccess : IDataAccess
    {
        private readonly IConfiguration configuration;
        private readonly string dbconnection;
        private readonly string dateformat;



        public DataAccess(IConfiguration configuration)
        {
            this.configuration = configuration;
            dbconnection = this.configuration["ConnectionStrings:DB"];
            dateformat = this.configuration["Constants:DateFormat"];
        }

        public Cart GetActiveCartOfUser(int userId)
        {
            var cart = new Cart();
            using SqlConnection connection = new(dbconnection);

            SqlCommand command = new()
            {
                Connection = connection,
            };
            connection.Open();

            string query = "select count(*) from Carts where UserId=" + userId + " and Ordered='false';";
            command.CommandText = query;

            int count = (int)command.ExecuteScalar();
            if (count == 0)
            {
                return cart;
            }
            query = "select CartId from Carts where UserId=" + userId + " and Ordered='false';";
            command.CommandText = query;

            int cartid = (int)command.ExecuteScalar();

            query = "select * from CartItems where CartId=" + cartid + ";";
            command.CommandText = query;

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                CartItem cartitem = new()
                {
                    Id = (int)reader["CartItemId"],
                    Product = GetProduct((int)reader["ProductId"])
                };
                cart.CartItems.Add(cartitem);
            }
            cart.Id = cartid;
            cart.User = GetUser(userId);
            cart.Ordered = false;
            cart.OrderedOn = "";

            return cart;
        }

        public List<Cart> GetAllPreviousCartsOfUser(int userid)
        {
            var carts = new List<Cart>();
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection,
            };
            string query = "select CartId from Carts where UserId=" + userid + " and Ordered='true';";
            command.CommandText = query;
            connection.Open();
            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                var cartid = (int)reader["cartId"];
                carts.Add(GetCart(cartid));
            }
            return carts;

        }

        public Cart GetCart(int cartid)
        {
            var cart = new Cart();
            using SqlConnection connection = new(dbconnection);
            SqlCommand cmd = new()
            {
                Connection = connection,
            };
            connection.Open();

            string query = "select * from CartItems where CartId=" + cartid + ";";
            cmd.CommandText = query;

            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                CartItem item = new()
                {
                    Id = (int)reader["CartItemId"],
                    Product = GetProduct((int)reader["ProductId"])
                };
                cart.CartItems.Add(item);
            }
            reader.Close();

            query = "select * from Carts where CartId=" + cartid + ";";
            cmd.CommandText = query;
            reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                cart.Id = cartid;
                cart.User = GetUser((int)reader["UserId"]);
                cart.Ordered = bool.Parse((string)reader["Ordered"]);
                cart.OrderedOn = (string)reader["OrderedOn"];
            }
            reader.Close();

            return cart;
        }

        public Offer GetOffer(int id)
        {
            var offer = new Offer();
            using SqlConnection conn = new(dbconnection);

            SqlCommand cmd = new()
            {
                Connection = conn,
            };
            string query = "Select * from Offers where OfferId=" + id + ";";
            cmd.CommandText = query;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                offer.Id = (int)reader["OfferId"];
                offer.Title = (string)reader["Title"];
                offer.Discount = (int)reader["Discount"];
            }
            return offer;
        }

        public Product GetProduct(int id)
        {
            var product = new Product();
            using SqlConnection conn = new(dbconnection);
            SqlCommand cmd = new()
            {
                Connection = conn,
            };
            string query = "select * from Products where ProductId=" + id + ";";
            cmd.CommandText = query;
            conn.Open();
            SqlDataReader r = cmd.ExecuteReader();
            while (r.Read())
            {
                product.Id = (int)r["ProductId"];
                product.Title = (string)r["Title"];
                product.Description = (string)r["Description"];
                product.Price = (double)r["Price"];
                product.Quantity = (int)r["Quantity"];
                product.ImageName = (string)r["ImageName"];

                var categoryId = (int)r["CategoryId"];
                product.ProductCategory = GetProductCategory(categoryId);

                var offerId = (int)r["OfferId"];
                product.Offer = GetOffer(offerId);
            }
            return product;
        }

        public bool UpdateProduct(int id)
        {
            using SqlConnection conn = new(dbconnection);
            SqlCommand cmd = new()
            {
                Connection = conn,
            };
            conn.Open();

            string query = "update Products set Quantity=Quantity-1 where ProductId=" + id + ";";
            cmd.CommandText = query;
            int rowsAffected = cmd.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public List<ProductCategory> GetProductCategories()
        {
            var productCategories = new List<ProductCategory>();
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection,
            };

            string query = "Select * from productCategories;";
            command.CommandText = query;

            connection.Open();
            SqlDataReader reader = command.ExecuteReader();

            while (reader.Read())
            {
                var category = new ProductCategory()
                {
                    Id = (int)reader["CategoryId"],
                    Category = (string)reader["Category"],
                    SubCategory = (string)reader["SubCategory"]
                };
                productCategories.Add(category);
            }
            return productCategories;
        }

        public ProductCategory GetProductCategory(int id)
        {
            var productCategory = new ProductCategory();
            using SqlConnection conn = new(dbconnection);

            SqlCommand cmd = new()
            {
                Connection = conn,
            };
            string query = "Select * from ProductCategories where CategoryId=" + id + ";";
            cmd.CommandText = query;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                productCategory.Id = (int)reader["CategoryId"];
                productCategory.Category = (string)reader["Category"];
                productCategory.SubCategory = (string)reader["SubCategory"];
            }
            return productCategory;
        }

        public List<Review> GetProductReviews(int productId)
        {
            var reviews = new List<Review>();
            using SqlConnection conn = new(dbconnection);
            SqlCommand cmd = new()
            { Connection = conn, };

            string query = "select * from Reviews where ProductId=  " + productId + ";";
            cmd.CommandText = query;

            conn.Open();
            SqlDataReader reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var review = new Review()
                {
                    Id = (int)reader["ReviewId"],
                    Value = (string)reader["Review"],
                    CreatedAt = (string)reader["CreatedAt"]
                };

                var userid = (int)reader["UserId"];
                review.User = GetUser(userid);

                var productid = (int)reader["ProductId"];
                review.Product = GetProduct(productid);

                reviews.Add(review);
            }
            return reviews;
        }

        public List<Product> GetProducts(string category, string subcategory, int count)
        {
            var products = new List<Product>();
            using SqlConnection connection = new(dbconnection);

            SqlCommand cmd = new SqlCommand("GetProducts", connection);
            cmd.CommandType = CommandType.StoredProcedure;

            cmd.Parameters.Add("@category", SqlDbType.NVarChar).Value = category;
            cmd.Parameters.Add("@subcategory", SqlDbType.NVarChar).Value = subcategory;
            cmd.Parameters.Add("@count", SqlDbType.Int).Value = count;

            connection.Open();
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var product = new Product()
                {
                    Id = (int)reader["ProductId"],
                    Title = (string)reader["Title"],
                    Description = (string)reader["Description"],
                    Price = (double)reader["Price"],
                    Quantity = (int)reader["Quantity"],
                    ImageName = (string)reader["ImageName"],
                };
                var categoryId = (int)reader["CategoryId"];
                product.ProductCategory = GetProductCategory(categoryId);

                var offerId = (int)reader["OfferId"];
                product.Offer = GetOffer(offerId);

                products.Add(product);
            }
            return products;
        }

        public User GetUser(int id)
        {
            var user = new User();
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection,
            };
            string query = "select * from Users where UserId=" + id + ";";
            command.CommandText = query;
            connection.Open();
            SqlDataReader r = command.ExecuteReader();
            while (r.Read())
            {
                user.Id = (int)r["UserId"];
                user.FirstName = (string)r["FirstName"];
                user.LastName = (string)r["LastName"];
                user.Email = (string)r["Email"];
                user.Address = (string)r["Address"];
                user.Mobile = (string)r["Mobile"];
                user.Password = (string)r["Password"];
                user.CreatedAt = (string)r["CreatedAt"];
                user.ModifiedAt = (string)r["ModifiedAt"];
            }

            return user;

        }

        public bool InsertCartItem(int userId, int productId)
        {
            using SqlConnection conn = new(dbconnection);
            conn.Open();
            SqlCommand cmd = new SqlCommand("InsertCartItem", conn);

            cmd.CommandType = CommandType.StoredProcedure;
            string query = "select count(*) from Carts where UserId=" + userId + "and Ordered='false';";
            cmd.Parameters.Add("@userId", SqlDbType.Int).Value = userId;
            cmd.Parameters.Add("@productId", SqlDbType.Int).Value = productId;

            cmd.ExecuteScalar();
            return true;
        }


        public bool RemoveCartItem(int userId, int productId)
        {
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    connection.Open();

                    string query = "SELECT CartId FROM Carts WHERE UserId = " + userId + " AND Ordered = 'false';";
                    cmd.CommandText = query;
                    int cartId = (int)cmd.ExecuteScalar();

                    if (cartId != 0)
                    {
                        query = "DELETE TOP(1) FROM CartItems WHERE CartId = " + cartId + " AND ProductId = " + productId + ";";
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }
        public bool EmptyCart(int userId)
        {
            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                using (SqlCommand cmd = new SqlCommand())
                {
                    cmd.Connection = connection;
                    connection.Open();

                    string query = "SELECT CartId FROM Carts WHERE UserId = " + userId + " AND Ordered = 'false';";
                    cmd.CommandText = query;
                    int cartId = (int)cmd.ExecuteScalar();

                    if (cartId != 0)
                    {
                        query = "DELETE FROM CartItems WHERE CartId = " + cartId + ";";
                        cmd.CommandText = query;
                        cmd.ExecuteNonQuery();
                    }
                }
            }

            return true;
        }


        public int InsertOrder(Order order)
        {
            int value = 0;

            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("InsertOrderProcedure", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = order.User.Id;
                    cmd.Parameters.Add("@cartId", SqlDbType.Int).Value = order.Cart.Id;
                    cmd.Parameters.Add("@paymentId", SqlDbType.Int).Value = order.Payment.Id;
                    cmd.Parameters.Add("@createdAt", SqlDbType.NVarChar).Value = order.CreatedAt;

                    value = (int)cmd.ExecuteScalar();
                }
            }

            return value;
        }

        public int InsertPayment(Payment payment)
        {
            int value = 0;

            using (SqlConnection connection = new SqlConnection(dbconnection))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand("InsertPaymentProcedure", connection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@userId", SqlDbType.Int).Value = payment.User.Id;
                    cmd.Parameters.Add("@totalAmount", SqlDbType.Decimal).Value = payment.TotalAmount;
                    cmd.Parameters.Add("@shippingCharges", SqlDbType.Decimal).Value = payment.ShippingCharges;
                    cmd.Parameters.Add("@amountReduced", SqlDbType.Decimal).Value = payment.AmountReduced;
                    cmd.Parameters.Add("@amountPaid", SqlDbType.Decimal).Value = payment.AmountPaid;
                    cmd.Parameters.Add("@createdAt", SqlDbType.NVarChar).Value = payment.CreatedAt;

                    value = (int)cmd.ExecuteScalar();
                }
            }
            return value;
        }

        public void InsertReview(Review review)
        {
            using SqlConnection connection = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = connection,
            };

            string query = "insert into Reviews(UserId, ProductId, Review, CreatedAt) values (@uid, @pid, @rv, @cat);";

            command.CommandText = query;
            command.Parameters.Add("@uid", System.Data.SqlDbType.Int).Value = review.User.Id;
            command.Parameters.Add("@pid", System.Data.SqlDbType.Int).Value = review.Product.Id;
            command.Parameters.Add("@rv", System.Data.SqlDbType.NVarChar).Value = review.Value;
            command.Parameters.Add("@cat", System.Data.SqlDbType.NVarChar).Value = review.CreatedAt;

            connection.Open();
            command.ExecuteNonQuery();
        }

        public bool InsertUser(User user)
        {
            using SqlConnection conn = new(dbconnection);
            conn.Open();
            SqlCommand cmd = new SqlCommand("InsertUserProcedure", conn);
            cmd.CommandType = CommandType.StoredProcedure;

            string hashedPassword = ComputeHash(user.Password);

            cmd.Parameters.Add("@firstName", SqlDbType.NVarChar).Value = user.FirstName;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = user.LastName;
            cmd.Parameters.Add("@address", SqlDbType.NVarChar).Value = user.Address;
            cmd.Parameters.Add("@mobile", SqlDbType.NVarChar).Value = user.Mobile;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = user.Email;
            cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = hashedPassword;
            cmd.Parameters.Add("@createdAt", SqlDbType.NVarChar).Value = user.CreatedAt;
            cmd.Parameters.Add("@modifiedAt", SqlDbType.NVarChar).Value = user.ModifiedAt;



            cmd.ExecuteScalar();
            return true;
        }

        public bool UpdateUser(User user)
        {
            using SqlConnection conn = new(dbconnection);
            conn.Open();
            SqlCommand cmd = new SqlCommand("UpdateUserProcedure", conn);

            cmd.CommandType = CommandType.StoredProcedure;

            user.Password = ComputeHash(user.Password);

            cmd.Parameters.Add("@firstName", SqlDbType.NVarChar).Value = user.FirstName;
            cmd.Parameters.Add("@lastName", SqlDbType.NVarChar).Value = user.LastName;
            cmd.Parameters.Add("@address", SqlDbType.NVarChar).Value = user.Address;
            cmd.Parameters.Add("@mobile", SqlDbType.NVarChar).Value = user.Mobile;
            cmd.Parameters.Add("@email", SqlDbType.NVarChar).Value = user.Email;
            cmd.Parameters.Add("@password", SqlDbType.NVarChar).Value = user.Password;
            cmd.Parameters.Add("@modifiedAt", SqlDbType.NVarChar).Value = user.ModifiedAt;

            cmd.ExecuteScalar();

            return true;
        }


        public string IsUserPresent(string email, string password)
        {
            User user = new();
            using SqlConnection conn = new(dbconnection);
            SqlCommand command = new()
            {
                Connection = conn,
            };
            conn.Open();

            string hashPassword = ComputeHash(password);
            string query = "SELECT COUNT(*) FROM Users WHERE Email='" + email + "' AND Password='" + hashPassword + "';";
            command.CommandText = query;
            int count = (int)command.ExecuteScalar();
            if (count == 0)
            {
                conn.Close();
                return "";
            }

            query = "select * from Users where Email='" + email + "'and Password='" + hashPassword + "';";
            command.CommandText = query;

            SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                user.Id = (int)reader["UserId"];
                user.FirstName = (string)reader["FirstName"];
                user.LastName = (string)reader["LastName"];
                user.Email = (string)reader["Email"];
                user.Address = (string)reader["Address"];
                user.Mobile = (string)reader["Mobile"];
                user.Password = (string)reader["Password"];
                user.CreatedAt = (string)reader["CreatedAt"];
                user.ModifiedAt = (string)reader["ModifiedAt"];
            }
            string key = "PFZNBnnnlOGSNbynKqZfxX0tZzjz8zfG";
            string duration = "60";
            var symmetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
            var credentials = new SigningCredentials(symmetricKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]{
                new Claim("id", user.Id.ToString()),
                new Claim("firstName", user.FirstName),
                new Claim("lastName", user.LastName.ToString()),
                new Claim("address", user.Address),
                new Claim("mobile", user.Mobile),
                new Claim("email", user.Email),
                new Claim("createdAt", user.CreatedAt),
                new Claim("modifiedAt", user.ModifiedAt),

            };

            var jwtToken = new JwtSecurityToken(
                issuer: "localhost",
                audience: "localhost",
                claims: claims,
                expires: DateTime.Now.AddMinutes(int.Parse(duration)),
                signingCredentials: credentials
                );
            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }

        public static string ComputeHash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] inputBytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(inputBytes);

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    builder.Append(hashBytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }


    }
}

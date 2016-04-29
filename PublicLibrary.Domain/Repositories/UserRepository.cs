using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PublicLibrary.Domain
{
    public class UserRepository
    {

        private string ConnString { get; set; }

        public UserRepository(string ConnectoinString)
        {
            ConnString = ConnectoinString; 
        }

        public string GetConnectionString()
        {
            //const string MyConnectoinString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionStringName"].ConnectionString;

            //string ConnString = "Server=192.168.10.220;Database=_TEST_PublicLibrary1;Uid=sa;Pwd=Undergr0und;";

            return ConnString;
        }

        public void AddUser(User User)
        {
            bool Exist = false;

            try
            {
                Exist = IfExistUser(User);
            }
            catch (DataBaseAlreadyHasElementException<User> e)
            {
                Exist = true;
            }


            if (Exist)
            {
                return;
            }

            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                Conn.Open();

                SqlDataAdapter Adaptor = new SqlDataAdapter();

                string CommandText = "insert into dbo.Users (FirstName, LastName, Sex, EMail, Password, BornDate, Age) values(@FirstName, @LastName, @Sex, @Email, @Password, @BornDate, @Age)";

                Adaptor.InsertCommand = new SqlCommand(CommandText, Conn);
                Adaptor.InsertCommand.CommandType = CommandType.Text;

                Adaptor.InsertCommand.Parameters.Clear();

                Adaptor.InsertCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@Sex", SqlDbType.SmallInt);
                Adaptor.InsertCommand.Parameters.Add("@Email", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@Password", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@BornDate", SqlDbType.Date);
                Adaptor.InsertCommand.Parameters.Add("@Age", SqlDbType.Int);

                Adaptor.InsertCommand.Parameters["@FirstName"].Value = User.FirstName;
                Adaptor.InsertCommand.Parameters["@LastName"].Value = User.LastName;
                Adaptor.InsertCommand.Parameters["@Sex"].Value = (int)User.Sex;
                Adaptor.InsertCommand.Parameters["@Email"].Value = User.EMail;
                Adaptor.InsertCommand.Parameters["@Password"].Value = User.Password;
                Adaptor.InsertCommand.Parameters["@BornDate"].Value = User.BornDate;
                Adaptor.InsertCommand.Parameters["@Age"].Value = User.Age;

                Adaptor.InsertCommand.ExecuteNonQuery();

                User.Id = GetUser(User.FirstName, User.LastName).Id;

                Conn.Close();
            }
        }

        public bool IfExistUser(User User)
        {
            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from Users where FirstName = @FirstName AND LastName = @LastName";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@FirstName"].Value = User.FirstName;
                Adaptor.SelectCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@LastName"].Value = User.LastName;

                Adaptor.Fill(ds);

                DataTable Users = ds.Tables[0];

                var query = Users.AsEnumerable().Select(user => new
                {
                    UserName = user.Field<string>("FirstName"),
                    UserLastName = user.Field<string>("LastName"),
                    UserID = user.Field<int>("ID")
                });

                query.Where(cond => cond.UserName == User.FirstName && cond.UserLastName == User.LastName);

                Conn.Close();

                if (query.Count() > 0)
                {
                    throw new DataBaseAlreadyHasElementException<User>(User);
                }
            }

            return false;
        }

        public User GetUser(int ID)
        {
            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from Users where ID = @ID";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.SelectCommand.Parameters["@ID"].Value = ID;

                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new User
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime>("BornDate"),
                        EMail = dataRow.Field<string>("EMail"),
                        Age = dataRow.Field<int>("Age"),
                        Id = dataRow.Field<int>("ID")
                    })
                    .ToList();

                if (empList.Count > 0)
                {
                    return empList[0];
                }

                Conn.Close();
            }

            return null;
        }

        public User GetUser(string FirstName, string LastName)
        {
            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from Users where FirstName = @FirstName AND LastName = @LastName";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@FirstName"].Value = FirstName;

                Adaptor.SelectCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@LastName"].Value = LastName;

                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new User
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime>("BornDate"),
                        EMail = dataRow.Field<string>("EMail"),
                        Age = dataRow.Field<int>("Age"),
                        Id = dataRow.Field<int>("ID")
                    })
                    .ToList();

                if (empList.Count > 0)
                {
                    return empList[0];
                }

                Conn.Close();
            }

            return null;
        }

        public IEnumerable<User> GetUsers()
        {
            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from Users";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new User
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime?>("BornDate"),
                        EMail = dataRow.Field<string>("EMail"),
                        Age = dataRow.Field<int>("Age"),
                        Id = dataRow.Field<int>("ID")
                    })
                    .ToList();

                if (empList.Count > 0)
                {
                    return empList;
                }

                Conn.Close();
            }

            return new List<User>();
        }

        public void EditUser(User user)
        {
            using (SqlConnection Conn = new SqlConnection(ConnString))
            {
                Conn.Open();

                SqlDataAdapter Adaptor = new SqlDataAdapter();

                string CommandText = "";

                CommandText = @"UPDATE [dbo].[Users]
                SET [EMail] = @EMail
                  ,[Age] = @Age
                  ,[Sex] = @Sex
                  ,[FirstName] = @FirstName
                  ,[LastName] = @LastName
                  ,[BornDate] = @BornDate
                 WHERE ID = @ID";


                Adaptor.UpdateCommand = new SqlCommand(CommandText, Conn);
                Adaptor.UpdateCommand.CommandType = CommandType.Text;

                Adaptor.UpdateCommand.Parameters.Clear();

                Adaptor.UpdateCommand.Parameters.Add("@EMail", SqlDbType.NVarChar);
                Adaptor.UpdateCommand.Parameters["@EMail"].Value = user.EMail;

                Adaptor.UpdateCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.UpdateCommand.Parameters["@FirstName"].Value = user.FirstName;

                Adaptor.UpdateCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.UpdateCommand.Parameters["@LastName"].Value = user.LastName;

                Adaptor.UpdateCommand.Parameters.Add("@Sex", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@Sex"].Value = user.Sex;

                Adaptor.UpdateCommand.Parameters.Add("@BornDate", SqlDbType.Date);
                Adaptor.UpdateCommand.Parameters["@BornDate"].Value = user.BornDate;

                Adaptor.UpdateCommand.Parameters.Add("@Age", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@Age"].Value = user.Age;

                Adaptor.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@ID"].Value = user.Id;

                Adaptor.UpdateCommand.ExecuteNonQuery();

                Conn.Close();
            }
        }

        public void DeleteUser(int Id)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                Conn.Open();

                SqlDataAdapter Adaptor = new SqlDataAdapter();

                string CommandText = "DELETE dbo.Users WHERE ID = @ID";

                Adaptor.DeleteCommand = new SqlCommand(CommandText, Conn);
                Adaptor.DeleteCommand.CommandType = CommandType.Text;

                Adaptor.DeleteCommand.Parameters.Clear();

                Adaptor.DeleteCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.DeleteCommand.Parameters["@ID"].Value = Id;

                Adaptor.DeleteCommand.ExecuteNonQuery();

                Conn.Close();
            }
        }
    }
}


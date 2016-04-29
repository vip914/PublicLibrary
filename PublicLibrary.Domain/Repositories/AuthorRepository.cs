using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace PublicLibrary.Domain
{
    public class AuthorRepository
    {
        private string ConnString { get; set; }

        public AuthorRepository(string ConnectoinString)
        {
            ConnString = ConnectoinString;
        }

        public string GetConnectionString()
        {
            //const string MyConnectoinString = System.Configuration.ConfigurationManager.ConnectionStrings["connectionStringName"].ConnectionString;

            //string ConnString = "Server=192.168.10.220;Database=_TEST_PublicLibrary1;Uid=sa;Pwd=Undergr0und;";

            return ConnString;
        }

       

        public void AddAuthor(Author Author)
        {
            bool Exist = false;

            try
            {
                Exist = IfExistAuthor(Author);
            }
            catch (DataBaseAlreadyHasElementException<Author> e)
            {
                Exist = true;
            }


            if (Exist)
            {
                return;
            }

            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                Conn.Open();

                string CommandText;

                SqlDataAdapter Adaptor = new SqlDataAdapter();
                if (Author.DeathDate != null)
                {
                    CommandText = "insert into dbo.Authors (FirstName, LastName, Sex, BornDate, Age, DeathDate) values(@FirstName, @LastName, @Sex, @BornDate, @Age, @DeathDate)";
                }
                else
                {
                    CommandText = "insert into dbo.Authors (FirstName, LastName, Sex, BornDate, Age) values(@FirstName, @LastName, @Sex, @BornDate, @Age)";
                }

                Adaptor.InsertCommand = new SqlCommand(CommandText, Conn);
                Adaptor.InsertCommand.CommandType = CommandType.Text;

                Adaptor.InsertCommand.Parameters.Clear();

                Adaptor.InsertCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.InsertCommand.Parameters.Add("@Sex", SqlDbType.SmallInt);
                Adaptor.InsertCommand.Parameters.Add("@BornDate", SqlDbType.Date);
                Adaptor.InsertCommand.Parameters.Add("@Age", SqlDbType.Int);
                if (Author.DeathDate != null)
                {
                    Adaptor.InsertCommand.Parameters.Add("@DeathDate", SqlDbType.Date);
                }


                Adaptor.InsertCommand.Parameters["@FirstName"].Value = Author.FirstName;
                Adaptor.InsertCommand.Parameters["@LastName"].Value = Author.LastName;
                Adaptor.InsertCommand.Parameters["@Sex"].Value = Author.Sex;
                Adaptor.InsertCommand.Parameters["@BornDate"].Value = Author.BornDate;
                Adaptor.InsertCommand.Parameters["@Age"].Value = Author.Age;
                if (Author.DeathDate != null)
                {
                    Adaptor.InsertCommand.Parameters["@DeathDate"].Value = Author.DeathDate;
                }

                Adaptor.InsertCommand.ExecuteNonQuery();

                Author.Id = GetAuthor(Author.FirstName, Author.LastName).Id;

                Conn.Close();
            }
        }

        public bool IfExistAuthor(Author Author)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from Authors where FirstName = @FirstName AND LastName = @LastName";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@FirstName"].Value = Author.FirstName;
                Adaptor.SelectCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.SelectCommand.Parameters["@LastName"].Value = Author.LastName;

                Adaptor.Fill(ds);

                DataTable Authors = ds.Tables[0];

                var query = Authors.AsEnumerable().Select(author => new
                {
                    FirstName = author.Field<string>("FirstName"),
                    LastName = author.Field<string>("LastName"),
                    ID = author.Field<int>("ID")
                });

                query.Where(cond => cond.FirstName == Author.FirstName && cond.LastName == Author.LastName);

                if (query.Count() > 0)
                {
                    throw new DataBaseAlreadyHasElementException<Author>(Author);
                }

            }

            return false;
        }

        public Author GetAuthor(int ID)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from dbo.Authors where ID = @ID";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.SelectCommand.Parameters["@ID"].Value = ID;

                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new Author
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime>("BornDate"),
                        DeathDate = dataRow.Field<DateTime>("DeathDate"),
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

        public Author GetAuthor(string FirstName, string LastName)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from dbo.Authors where FirstName = @FirstName AND LastName = @LastName";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);

                Adaptor.SelectCommand.Parameters.Clear();

                Adaptor.SelectCommand.Parameters.Add("@FirstName", SqlDbType.NChar);
                Adaptor.SelectCommand.Parameters["@FirstName"].Value = FirstName;

                Adaptor.SelectCommand.Parameters.Add("@LastName", SqlDbType.NChar);
                Adaptor.SelectCommand.Parameters["@LastName"].Value = LastName;

                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new Author
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime>("BornDate"),
                        DeathDate = dataRow.Field<DateTime?>("DeathDate"),
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

        public IEnumerable<Author> GetAuthors()
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                SqlDataAdapter Adaptor = new SqlDataAdapter();

                DataSet ds = new DataSet();

                string CommandText = "select * from dbo.Authors";

                Adaptor.SelectCommand = new SqlCommand(CommandText, Conn);
                                
                Adaptor.Fill(ds);

                var empList = ds.Tables[0].AsEnumerable()
                    .Select(dataRow => new Author
                    {
                        FirstName = dataRow.Field<string>("FirstName"),
                        LastName = dataRow.Field<string>("LastName"),
                        Sex = dataRow.Field<User.MaleFemale>("Sex"),
                        BornDate = dataRow.Field<DateTime>("BornDate"),
                        DeathDate = dataRow.Field<DateTime>("DeathDate"),
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

            return new List<Author>();
        }

        public void EditAuthor(Author author)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                Conn.Open();

                SqlDataAdapter Adaptor = new SqlDataAdapter();

                string CommandText = "";

                if (author.DeathDate != null)
                {
                    CommandText = @"UPDATE dbo.Authors
                    SET [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Sex] = @Sex
                        ,[BornDate] = @BornDate
                        ,[Age] = @Age
                        ,[DeathDate] = @DeathDate
                    WHERE ID = @ID";
                }
                else
                {
                    CommandText = @"UPDATE dbo.Authors
                    SET [FirstName] = @FirstName
                        ,[LastName] = @LastName
                        ,[Sex] = @Sex
                        ,[BornDate] = @BornDate
                        ,[Age] = @Age
                    WHERE ID = @ID";
                }

                Adaptor.UpdateCommand = new SqlCommand(CommandText, Conn);
                Adaptor.UpdateCommand.CommandType = CommandType.Text;

                Adaptor.UpdateCommand.Parameters.Clear();

                Adaptor.UpdateCommand.Parameters.Add("@FirstName", SqlDbType.NVarChar);
                Adaptor.UpdateCommand.Parameters["@FirstName"].Value = author.FirstName;

                Adaptor.UpdateCommand.Parameters.Add("@LastName", SqlDbType.NVarChar);
                Adaptor.UpdateCommand.Parameters["@LastName"].Value = author.LastName;

                Adaptor.UpdateCommand.Parameters.Add("@Sex", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@Sex"].Value = author.Sex;

                Adaptor.UpdateCommand.Parameters.Add("@BornDate", SqlDbType.Date);
                Adaptor.UpdateCommand.Parameters["@BornDate"].Value = author.BornDate;

                Adaptor.UpdateCommand.Parameters.Add("@Age", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@Age"].Value = author.Age;

                Adaptor.UpdateCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.UpdateCommand.Parameters["@ID"].Value = author.Id;

                if (author.DeathDate != null)
                {
                    Adaptor.UpdateCommand.Parameters.Add("@DeathDate", SqlDbType.Date);
                    Adaptor.UpdateCommand.Parameters["@DeathDate"].Value = author.DeathDate;
                }

                Adaptor.UpdateCommand.ExecuteNonQuery();

                Conn.Close();
            }
        }

        public void DeleteAuthor(int ID)
        {
            using (SqlConnection Conn = new SqlConnection(GetConnectionString()))
            {
                Conn.Open();

                SqlDataAdapter Adaptor = new SqlDataAdapter();

                string CommandText = "DELETE dbo.Authors WHERE ID = @ID";

                Adaptor.DeleteCommand = new SqlCommand(CommandText, Conn);
                Adaptor.DeleteCommand.CommandType = CommandType.Text;

                Adaptor.DeleteCommand.Parameters.Clear();

                Adaptor.DeleteCommand.Parameters.Add("@ID", SqlDbType.Int);
                Adaptor.DeleteCommand.Parameters["@ID"].Value = ID;

                Adaptor.DeleteCommand.ExecuteNonQuery();

                Conn.Close();
            }
        }


    }
}

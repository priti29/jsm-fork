using jsm33t.com.Models.View;
using jsm33t.com.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace jsm33t.com.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class LoginModel : PageModel
    {
        public Login? LoginCreds{ get; set; }
        public void OnGet()
        {
            var sessionactivity = HttpContext.Session.GetString("username");
            if (sessionactivity != null)
            {
                Response.Redirect("/account");
            }

        }
        public async Task<JsonResult> OnPostLoginInit([FromBody] Login LoginCreds)
        {
            string connectionString = ConfigHelper.NewConnectionString;
            string message = "something went wrong", type = "error";
            if (LoginCreds.UserName != null && LoginCreds.PassWord!= null)
            {
                if (LoginCreds.UserName== "")
                {
                    message = "username is mandatory";
                    type = "error";
                }
                else
                {
                    try
                    {
                        using SqlConnection connection = new(connectionString);
                        await connection.OpenAsync();

                        SqlCommand checkcommand= new("SELECT * from TblUserProfile WHERE UserName = @username and PassWord =@password", connection);
                        checkcommand.Parameters.AddWithValue("@username",LoginCreds.UserName);
                        checkcommand.Parameters.AddWithValue("@password", LoginCreds.PassWord);
                        //await checkcommand.ExecuteScalarAsync();
                        using (var reader = await checkcommand.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                               
                               var username = reader.GetString(reader.GetOrdinal("UserName"));
                               var firstname = reader.GetString(reader.GetOrdinal("FirstName"));
                                var role = reader.GetString(reader.GetOrdinal("Role"));
                                var avatar = reader.GetString(reader.GetOrdinal("Role"));
                                HttpContext.Session.SetString("username", username);
                                HttpContext.Session.SetString("first_name", firstname);
                                HttpContext.Session.SetString("role", role);
                                // HttpContext.Session.SetString("avatar", role);
                                //Response.Redirect("/account");
                                message = "successfully logged in";
                                type = "success";

                            }
                            else
                            {
                                message = "something went wrongno record found";
                                type = "error";
                            }
                        }
                        await connection.CloseAsync();
                    }
                    catch(Exception ex)
                    {
                        message = "something went wrong" + ex.ToString();
                        type = "error";
                    }
                }
            }
            else
            {
                message = "something went wrong:";
                type = "error";
            }
            var keys = new { message, type };
            return new JsonResult(keys);
        }
    }
}

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

                        SqlCommand checkcommand= new("select * from TblUserProfile where UserName = @username and PassWord =@password", connection);
                        checkcommand.Parameters.AddWithValue("@username",LoginCreds.UserName);
                        checkcommand.Parameters.AddWithValue("@password", LoginCreds.PassWord);
                        using (var reader = await checkcommand.ExecuteReaderAsync())
                        {
                            if (reader.Read())
                            {
                               
                               var username = reader.GetString(reader.GetOrdinal("UserName"));
                               var firstname = reader.GetString(reader.GetOrdinal("FirstName"));
                                var role = reader.GetString(reader.GetOrdinal("Role"));
                                var avatar = reader.GetInt32(reader.GetOrdinal("AvatarId"));
                                HttpContext.Session.SetString("username", username);
                                HttpContext.Session.SetString("first_name", firstname);
                                HttpContext.Session.SetString("role", role);
                                HttpContext.Session.SetString("avatar", avatar.ToString());
                                //Response.Redirect("/account");
                                message = "logging in...";
                                type = "success";

                            }
                            else
                            {
                                message = "Invalid Credentials";
                                type = "error";
                            }
                        }
                        await connection.CloseAsync();
                    }
                    catch(Exception ex)
                    {
                        if (HttpContext.Session.GetString("role") == "admin")
                        { message = "something went wrong " + ex.Message.ToString(); }
                        else
                        { message = "something went wrong"; }
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

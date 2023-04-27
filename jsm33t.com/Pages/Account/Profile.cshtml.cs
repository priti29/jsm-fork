using jsm33t.com.Models.View;
using jsm33t.com.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using System.Data.SqlClient;

namespace jsm33t.com.Pages.Account
{
    [IgnoreAntiforgeryToken]
    public class ProfileModel : PageModel
    {
        public UserDeets? UserDetailsDisplay { get; set; }

        public List<AvatarDropdown>? AvatarDD { get; set; }
        public void OnGet()
        {
            if (HttpContext.Session.GetString("username") == null)
            {
                Response.Redirect("/");
            }
            else
            {
                LoadUserData();
                LoadAvatarDropdown();
                string fname = "", lname = "", role = "";
                if (UserDetailsDisplay != null)
                {
                    if (UserDetailsDisplay.FirstName != null)
                    { fname = UserDetailsDisplay.FirstName.ToString(); }
                    else
                    { fname = ""; }
                    if (UserDetailsDisplay.LastName != null)
                    { lname = UserDetailsDisplay.LastName.ToString(); }
                    else
                    { lname = ""; }
                    if (UserDetailsDisplay.Role != null)
                    { role = UserDetailsDisplay.Role.ToString(); }
                }

                ViewData["role"] = role;
                ViewData["fullname"] = fname + " " + lname;
            }

        }
        public void OnPost()
        {
        
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0058:Expression value is never used", Justification = "<Pending>")]
        public void LoadUserData()
        {
            string connectionString = ConfigHelper.NewConnectionString;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM TblUserProfile WHERE UserName = @Id", connection);
            command.Parameters.AddWithValue("@Id", HttpContext.Session.GetString("username"));
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                UserDetailsDisplay = new UserDeets()
                {
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    UserName = reader.GetString(3),
                    Role = reader.GetString(11),
                    EMail = reader.GetString(4),
                    Phone = reader.GetString(5),
                    Gender = reader.GetString(6),
                    Bio = reader.GetString(12)
                };
            }
            else
            {
                //return NotFound();
            }
            connection.Close();

        }

        public void LoadAvatarDropdown()
        {
            AvatarDD = new List<AvatarDropdown>();
            string connectionString = ConfigHelper.NewConnectionString;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM TblAvatarMaster ", connection);
            var reader = command.ExecuteReader();

            while (reader.Read())
            {
                AvatarDD.Add(new AvatarDropdown
                {
                    Id = reader.GetInt32(0),
                    Title = reader.GetString(1),
                    Image = reader.GetString(2),
                    Description = reader.GetString(3)
                });
            }
            reader.Close();

        }

        public async Task<JsonResult> OnPostSubmitDeets([FromBody] UserDeets EditProfile)
        {
            string connectionString = ConfigHelper.NewConnectionString;
            string message = "something went wrong", type = "error";
            if (EditProfile.FirstName != null && EditProfile.LastName != null)
            {
                if (EditProfile.FirstName == "")
                {
                    message = "first name is mandatory";
                    type = "error";
                }
                else if (EditProfile.EMail == "")
                {
                    message = "email name is mandatory";
                    type = "error";
                }
                else
                {
                    try
                    {
                        using SqlConnection connection = new(connectionString);

                        await connection.OpenAsync();
                        SqlCommand insertCommand = new("UPDATE TblUserProfile SET FirstName = @FirstName,LastName = @LastName,EMail = @EMail,Phone = @Phone,AvatarId= @AvatarId,Gender = @Gender where UserName = @UserName", connection);
                        insertCommand.Parameters.AddWithValue("@FirstName", EditProfile.FirstName.Trim());
                        insertCommand.Parameters.AddWithValue("@LastName", EditProfile.LastName.Trim());
                        insertCommand.Parameters.AddWithValue("@EMail", EditProfile.EMail.Trim());
                        insertCommand.Parameters.AddWithValue("@Phone", EditProfile.Phone.Trim());
                        insertCommand.Parameters.AddWithValue("@Gender", EditProfile.Gender.Trim());
                        insertCommand.Parameters.AddWithValue("@AvatarId", EditProfile.AvatarId);
                        insertCommand.Parameters.AddWithValue("@UserName", HttpContext.Session.GetString("username"));
                        await insertCommand.ExecuteNonQueryAsync();
                        message = "Changes Saved!!";
                        type = "success";
                        await connection.CloseAsync();

                        if (HttpContext.Session.GetString("username") != null)
                        {
                            HttpContext.Session.SetString("first_name", EditProfile.FirstName.Trim());
                            HttpContext.Session.SetString("avatar", EditProfile.AvatarId.ToString().Trim());
                           


                            // HttpContext.Session.SetString("role", role);
                            // HttpContext.Session.SetString("avatar", role);
                        }
                    }
                    catch
                    {
                        message = "something went wrong";
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

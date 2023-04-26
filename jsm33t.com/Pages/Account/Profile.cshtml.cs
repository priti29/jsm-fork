using jsm33t.com.Models.View;
using jsm33t.com.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
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
        public void OnPost() { }

        public void LoadUserData()
        {
            string connectionString = ConfigHelper.NewConnectionString;
            using var connection = new SqlConnection(connectionString);
            connection.Open();
            var command = new SqlCommand("SELECT * FROM TblUserProfile WHERE UserName = @Id", connection);
            command.Parameters.AddWithValue("@Id", "jsm33t");
            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                UserDetailsDisplay = new UserDeets()
                {
                    FirstName = reader.GetString(1),
                    LastName = reader.GetString(2),
                    UserName = reader.GetString(3),
                    Role = reader.GetString(11)
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
                else
                {
                    try
                    {
                        using SqlConnection connection = new(connectionString);

                        await connection.OpenAsync();
                        SqlCommand insertCommand = new("UPDATE TblUserProfile SET FirstName = @FirstName,LastName = @LastName where UserName = 'jsm33t'", connection);
                        insertCommand.Parameters.AddWithValue("@FirstName", EditProfile.FirstName.Trim());
                        insertCommand.Parameters.AddWithValue("@LastName", EditProfile.LastName.Trim());
                        await insertCommand.ExecuteNonQueryAsync();
                        message = "Changes Saved";
                        type = "success";
                        await connection.CloseAsync();
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

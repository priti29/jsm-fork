using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using jsm33t.com.Modules;
using System.Data.SqlClient;
using System.Net.Mail;

namespace jsm33t.com.Pages
{
    [ValidateAntiForgeryToken]
    public class IndexModel : PageModel
    {

        public void OnGet()
        {

        }
        public void OnPost()
        {

        }
        public bool IsValidEmail(string email)
        {
            try
            {
                var mailAddress = new MailAddress(email);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        public async Task<JsonResult> OnPostSubmitMailAsync(string mailSub)
        {
            string connectionString = ConfigHelper.NewConnectionString;

            //string applicationpath = HttpContext.Request.Host + HttpContext.Request.Path;
            string applicationpath = HttpContext.Request.Path;
            string message, type = "error";
            try
            {
                if (!IsValidEmail(mailSub.Trim()))
                {
                    message = "invalid email format";
                    type = "error";
                }
                else if (mailSub.Trim() == "")
                {
                    message = "please fillup the email";
                    type = "error";
                }
                else
                {
                    using SqlConnection connection = new(connectionString);
                    SqlCommand selectCommand = new("SELECT COUNT(*) FROM TblMailingList WHERE Email = @Column1", connection);
                    selectCommand.Parameters.AddWithValue("@Column1", mailSub);
                    await connection.OpenAsync();
                    int count = (int)selectCommand.ExecuteScalar();
                    if (count == 0)
                    {
                        SqlCommand insertCommand = new("INSERT INTO TblMailingList (Email, Origin,DateAdded) VALUES (@Column1, @Column2,@Column3)", connection);
                        insertCommand.Parameters.AddWithValue("@Column1", mailSub);
                        insertCommand.Parameters.AddWithValue("@Column2", applicationpath);
                        insertCommand.Parameters.AddWithValue("@Column3", DateTime.Now);
                        await insertCommand.ExecuteNonQueryAsync();
                        message = "email submitted";
                        type = "success";
                    }
                    else
                    {
                        message = "email already present";
                        type = "error";
                    }
                    await connection.CloseAsync();
                }

            }
            catch (Exception ex)
            {
                message = "something went wrong:" + ex.Message;
                type = "error";
            }

            var keys = new { message, type };
            return new JsonResult(keys);
        }
    }
}
using jsm33t.com.Models.View;
using jsm33t.com.Modules;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data.SqlClient;

namespace jsm33t.com.Pages.Blog
{
    public class HomeModel : PageModel
    {
        public BlogThumbs? BlogDisplay { get; set; }
        public void OnGet()
        {
            //await LoadLatestBlogsAsync();
        }

        //protected async Task LoadLatestBlogsAsync()
        //{
           
        //        string connectionString = ConfigHelper.NewConnectionString;
        //        using var connection = new SqlConnection(connectionString);
        //        connection.Open();
        //        var command = new SqlCommand("SELECT * FROM TblBlogMaster", connection);
        //        var reader = await command.ExecuteReaderAsync();

        //        if (reader.Read())
        //        {
        //            BlogDisplay = new BlogThumbs()
        //            {
        //                Title = reader.GetString(1),
        //                Description = reader.GetString(2),
        //                UrlHandle = reader.GetString(2),
        //              //  Author = reader.GetString(11),
        //                Category = reader.GetString(4)
                        
        //            };
        //        }
        //        else
        //        {
        //            //return NotFound();
        //        }
        //        connection.Close();

        
        //}
    }
}

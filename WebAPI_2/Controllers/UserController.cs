using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Results;
using WebAPI_2.Models;
using System.Net.Http.Formatting;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;

namespace WebAPI_2.Controllers
{
    public class UserController : ApiController
    {
        // GET api/User/GetAllUser
        [HttpGet]
        public HttpResponseMessage GetAllUsers()
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/users.json"); //đường dẫn đến file JSON
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    string jsonData = reader.ReadToEnd();
                    var jsonObject = JObject.Parse(jsonData);
                    var userList = jsonObject["users"].ToObject<List<Users>>();
                    //var userList = JsonConvert.DeserializeObject<List<Users>>(jsonData);
                    return Request.CreateResponse(HttpStatusCode.OK, userList);
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
            }
        }
        private static readonly object fileLock = new object();
        // POST api/Users/CreateNew
        [HttpPost]
        public HttpResponseMessage CreateNewUser([FromBody] Users user)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/users.json");

            if (File.Exists(filePath))
            {
                // Read the JSON data from the file
                string jsonData = File.ReadAllText(filePath);

                // Parse the JSON data into a JObject
                JObject jsonObject = JObject.Parse(jsonData);

                // Get the "users" array from the JObject
                JArray usersArray = (JArray)jsonObject["users"];

                // Convert the user object to a JObject
                JObject newUser = JObject.FromObject(user);

                // Add the new user to the "users" array
                usersArray.Add(newUser);

                // Write the updated JSON data back to the file
                File.WriteAllText(filePath, jsonObject.ToString());

                return Request.CreateResponse(HttpStatusCode.OK, "User added successfully");
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
            }
        }

        // PUT api/Users/UpdateUser
        [HttpPut]
        public HttpResponseMessage UpdateUser([FromBody] Users user)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/users.json");
            if (!File.Exists(filePath))
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
            }

            var json = File.ReadAllText(filePath);
            var jsonObject = JObject.Parse(json);
            var userList = jsonObject["users"].ToObject<List<Users>>();

            var index = userList.FindIndex(u => u.Id == user.Id);
            if (index != -1)
            {
                userList[index].Id = user.Id;
                userList[index].Username = user.Username;
                userList[index].Password = user.Password;
                userList[index].Fullname = user.Fullname;
                userList[index].IsActive = user.IsActive;

                var updatedJson = JsonConvert.SerializeObject(new { users = userList });
                File.WriteAllText(filePath, updatedJson);
                return Request.CreateResponse(HttpStatusCode.OK, user);
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "User not found");
            }
        }

        [HttpDelete]
        public HttpResponseMessage DeleteUser([FromBody] Users user)
        {
            var filePath = HttpContext.Current.Server.MapPath("~/App_Data/users.json");
            if (File.Exists(filePath))
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    var json = reader.ReadToEnd();
                    var jsonObject = JObject.Parse(json);
                    var userList = jsonObject["users"].ToObject<List<Users>>();
                    var userToDelete = userList.FindIndex(u => u.Id == user.Id);

                    if (userToDelete >= 0)
                    {
                        userList.RemoveAt(userToDelete);
                        jsonObject["users"] = JToken.FromObject(userList);
                        string output = JsonConvert.SerializeObject(jsonObject, Formatting.Indented);

                        // Đóng StreamReader trước khi ghi vào tệp
                        reader.Close();

                        using (StreamWriter writer = new StreamWriter(filePath, false))
                        {
                            writer.Write(output);
                        }

                        return Request.CreateResponse(HttpStatusCode.OK, "User has been deleted.");
                    }
                    else
                    {
                        return Request.CreateErrorResponse(HttpStatusCode.NotFound, $"User with ID = {user.Id} not found.");
                    }
                }
            }
            else
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "File not found");
            }
        }



    }
}
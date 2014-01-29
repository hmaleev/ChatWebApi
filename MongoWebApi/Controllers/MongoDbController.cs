using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoWebApi.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace MongoWebApi.Controllers
{
    public class UserController : ApiController
    {
        public MongoDatabase Database;

        public UserController()
        {
            Database = RetreiveMongohqDb();
        }

        private MongoDatabase RetreiveMongohqDb()
        {
            const string uri = "mongodb://appharbor_7bf89f92-7605-4a7c-963e-b14e263e9abd:37r8l8rtj4j71f71dipff4df4f@ds063158.mongolab.com:63158/appharbor_7bf89f92-7605-4a7c-963e-b14e263e9abd";
            var client = new MongoClient(uri);
            return  client.GetServer().GetDatabase(new MongoUrl(uri).DatabaseName);
        }

        // GET api/GetAll
        [HttpGet ]
        public IEnumerable<User> GetAllUsers()
        {
            List<User> model = new List<User>();
            var usersList = Database.GetCollection("Users").FindAll().AsEnumerable();
            model = (from user in usersList
                     select new User
                     {
                         Id = user["_id"].ToString(),
                         Name = user["Name"].ToString(),
                         Password = user["Password"].ToString(),
                         IP = user["IP"].ToString()
                     }).ToList();
            return model;
        }


        [HttpGet]
        public IEnumerable<User> GetUser(string Name)
        {
            List<User> model = new List<User>();
            var usersList = Database.GetCollection("Users").FindAll().AsEnumerable();
            model = (from user in usersList
                     where user["Name"] == Name
                     select new User
                     {
                         Id = user["_id"].ToString(),
                         Name = user["Name"].ToString(),
                         Password = user["Password"].ToString(),
                         IP = user["IP"].ToString()

                     }).ToList();
            return model;
        }

        [HttpPost]
        public User CreateUser(User user)
        {
            var usersList = Database.GetCollection("Users");
            if (string.IsNullOrEmpty(user.Id))
            {
                user.Id = ObjectId.GenerateNewId().ToString();
               usersList.Insert<User>(user);

            }
            return user;
        }

        [HttpPut]
        public bool UpdateUser(string id,User user)
        {
             var users = Database.GetCollection<User>("Users"); 
            IMongoQuery query = Query.EQ("_id", id);
            IMongoUpdate update = Update
                .Set("Name", user.Name)
                .Set("Password", user.Password)
                .Set("IP", user.IP);

            WriteConcernResult result = users.Update(query, update); 
            return result.UpdatedExisting; 
        }

        [HttpDelete]
        public bool DeleteUser(string username)
        {
            var collection = Database.GetCollection<User>("Users");
            var query = Query<User>.EQ(e => e.Name, username);
            collection.Remove(query);
            return true;
        }
    }
}
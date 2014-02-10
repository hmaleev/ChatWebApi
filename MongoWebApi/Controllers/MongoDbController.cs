using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;
using MongoWebApi.Models;
using MongoDB.Bson.Serialization;
using System.Net.Http;
using System.Net;

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
                         IP = user["IP"].ToString(),
                         Contacts = BsonSerializer.Deserialize<List<User>>(user["Contacts"].ToJson())
                     }).ToList();

            return model;
        }


        [HttpGet]
        public IEnumerable<User> GetUser(string Name)
        {
            List<User> model = new List<User>();
            var usersList = Database.GetCollection("Users").FindAll().ToList();
            model = (from user in usersList
                     where user["Name"] == Name
                     select new User
                     {
                         Id = user["_id"].ToString(),
                         Name = user["Name"].ToString(),
                         Password = user["Password"].ToString(),
                         IP = user["IP"].ToString(),
                         Contacts = BsonSerializer.Deserialize<List<User>>( user["Contacts"].ToJson())
                     }).ToList();
            return model;
        }

        [HttpGet]
        public IEnumerable<User> FindUsers(string Name)
        {
          
            List<User> model = new List<User>();
            var usersList = Database.GetCollection("Users");

            var query = (from c in usersList.AsQueryable<User>()
                         where c.Name.Contains(Name)
                         select c);
            return query;
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

        [HttpPost]
        public User LogIn(User loggedUser)
        {
            User model = new User();
            var usersList = Database.GetCollection("Users").FindAll().AsEnumerable();
            model = (from user in usersList
                     where user["Name"] == loggedUser.Name && user["Password"] == loggedUser.Password

                     select new User
                     {
                         Id = user["_id"].ToString(),
                         Name = user["Name"].ToString(),
                         Password = user["Password"].ToString(),
                         IP = user["IP"].ToString()

                     }).FirstOrDefault();

            if (model==null)
            {
                return null;
            }
            else
            {
                return model;
            }
        }

        [HttpPut]
        public HttpResponseMessage UpdateUserIP(string name, [FromBody]User user)
        {

            var users = Database.GetCollection<User>("Users");
            var query = Query.And(
                Query.EQ("Name", name)
            );
            var sortBy = SortBy.Descending("Name");
            var update = Update
                .Set("IP", user.IP);
            var result = users.FindAndModify(
                query,
                sortBy,
                update,
                true // return new document
            );
            if (result != null)
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK, "Success");
                return response;
            }
            else
            {
                HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.BadRequest, "Error");
                return response;
            }
           
        }
        [HttpPut]
        public void AddContact(string name, [FromBody]User user)
        {

            var users = Database.GetCollection<User>("Users");
            var query = Query.And(
                Query.EQ("Name", name)
            );
            var sortBy = SortBy.Descending("Name");
            var update = Update<User>.Set(x => x.Contacts,user.Contacts);
            var result = users.FindAndModify(
                query,
                sortBy,
                update,
                true // return new document
            );
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
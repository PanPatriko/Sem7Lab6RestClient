using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab6RestClient
{
    class AuthenticateResponse
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }

        public AuthenticateResponse(int id, string username, string token)
        {
            Id = id;
            Username = username;
            Token = token;
        }
    }
}

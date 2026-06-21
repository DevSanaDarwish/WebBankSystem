using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebBankBusinessLayer
{
    public class Person
    {
            public int PersonID { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }

            public Person() { }

            public Person(int personID, string firstName, string lastName, string email)
            {
                this.PersonID = personID;
                this.FirstName = firstName;
                this.LastName = lastName;
                this.Email = email;
            }
        }
    }


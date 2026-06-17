using RoomReservation.Core.Data;
using RoomReservation.Core.Entities;
using RoomReservation.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace RoomReservation.Core.Seeders
{
    public class UserSeeder(AppDbContext _db) : ISeeder
    {
        public async Task SeedAsync()
        {
            List<User> users = new List<User>()
            {
                new() { Firstname = "Marek", Lastname = "Kowalski", Email = "marek.kowalski@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Anna", Lastname = "Nowak", Email = "anna.nowak@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Piotr", Lastname = "Wiśniewski", Email = "piotr.wisniewski@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Katarzyna", Lastname = "Wójcik", Email = "k.wojcik@interia.pl", PasswordHash = "password" },
                new() { Firstname = "Tomasz", Lastname = "Kamińska", Email = "tomasz.kaminska@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Magdalena", Lastname = "Lewandowski", Email = "m.lewandowski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Łukasz", Lastname = "Zielińska", Email = "lukasz.zielinska@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Agnieszka", Lastname = "Szymański", Email = "agnieszka.szymanski@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Michał", Lastname = "Woźniak", Email = "michal.wozniak@interia.pl", PasswordHash = "password" },
                new() { Firstname = "Joanna", Lastname = "Dąbrowski", Email = "joanna.dabrowski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Krzysztof", Lastname = "Kozłowska", Email = "k.kozlowska@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Monika", Lastname = "Jankowski", Email = "monika.jankowski@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Bartosz", Lastname = "Mazur", Email = "bartosz.mazur@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Ewelina", Lastname = "Kwiatkowski", Email = "ewelina.kwiatkowski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Rafał", Lastname = "Krawczyk", Email = "rafal.krawczyk@interia.pl", PasswordHash = "password" },
                new() { Firstname = "Paulina", Lastname = "Piotrowska", Email = "paulina.piotrowska@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Grzegorz", Lastname = "Grabowski", Email = "g.grabowski@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Natalia", Lastname = "Nowakowski", Email = "natalia.nowakowski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Damian", Lastname = "Pawłowski", Email = "damian.pawlowski@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Karolina", Lastname = "Michalska", Email = "k.michalska@interia.pl", PasswordHash = "password" },
                new() { Firstname = "Szymon", Lastname = "Adamczyk", Email = "szymon.adamczyk@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Aleksandra", Lastname = "Dudek", Email = "aleksandra.dudek@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Mateusz", Lastname = "Wieczorek", Email = "mateusz.wieczorek@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Dominika", Lastname = "Jabłońska", Email = "dominika.jablonska@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Marcin", Lastname = "Zawadzki", Email = "marcin.zawadzki@interia.pl", PasswordHash = "password" },
                new() { Firstname = "Sylwia", Lastname = "Majewski", Email = "sylwia.majewski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Paweł", Lastname = "Baran", Email = "pawel.baran@gmail.com", PasswordHash = "password" },
                new() { Firstname = "Izabela", Lastname = "Wróbel", Email = "izabela.wrobel@onet.pl", PasswordHash = "password" },
                new() { Firstname = "Kamil", Lastname = "Malinowski", Email = "kamil.malinowski@wp.pl", PasswordHash = "password" },
                new() { Firstname = "Weronika", Lastname = "Sikora", Email = "weronika.sikora@gmail.com", PasswordHash = "password" },
            };

            foreach (var user in users)
            {
                _db.Users.Add(user);
            }

            await _db.SaveChangesAsync();
        }
    }
}

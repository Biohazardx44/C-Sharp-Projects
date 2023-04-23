﻿using MovieStore.Domain.Enums;

namespace MovieStore.Domain.Entities
{
    public class Movie
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Year { get; set; }
        public Genre Genre { get; set; }
        private int _price { get; set; }

        public void SetPrice()
        {
            Random random = new Random();
            if (Year < 2000)
            {
                _price = random.Next(100, 201);
            }
            else if (Year >= 2000 && Year <= 2010)
            {
                _price = random.Next(200, 301);
            }
            else
            {
                _price = random.Next(300, 501);
            }
        }
    }
}
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TCPServer2
{
    public class Food
    {
        public enum FoodName
        {
            Tomato,
            Shrimp,
            Bacon,
            Onion,
            Bread
        }
        public FoodName name { get; set; }

        public Food(FoodName name)
        {
            this.name = name;
        }   

        public static string getName(int nameIndex)
        {
            FoodName foodName = (FoodName)nameIndex;

            // Convert the enum value to lowercase string
            return foodName.ToString();
        }

        public byte[] Convert_to_Data()
        {
            return Encoding.UTF8.GetBytes(this.name.ToString());
        }

        public static byte[] Convert_to_Data(FoodName _name)
        {
            return Encoding.UTF8.GetBytes(_name.ToString());
        }

        public static Food Convert_to_Food(byte[] raw_data)
        {
            int nameLength = Enum.GetNames(typeof(FoodName)).Length;
            string nameString = Encoding.UTF8.GetString(raw_data, 0, nameLength);
            FoodName name = (FoodName)Enum.Parse(typeof(FoodName), nameString);


            // Create and return Food object
            return new Food(name);
        }

    }
}

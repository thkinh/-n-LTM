using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

namespace Assets.Scripts.Game
{
    internal class Food
    {
        private int data = 0;
        public string name { get; } = string.Empty;

        private byte[] ReadytoSendData { get; } = new byte[4]; 
        


        public Food(string _name)
        {
            name = _name;
           switch(_name)
           {
                case "Tomato":
                    data = 1; break;
                case "Shrimp":
                    data = 2; break;
                case "Bacon":
                    data = 3; break;
           }
        }

        public Food(int _data)
        {
            data = _data;
            switch (_data)
            {
                case 1:
                    name = "Tomato";
                    break;
                case 2:
                    name = "Shrimp";
                    break;
                case 3:
                    name = "Bacon";
                    break;
            }
        }

        public static int Getdata(string name)
        {
            switch (name)
            {
                case "Tomato":
                    return 1;
                case "Shrimp":
                    return 2;
                case "Bacon":
                    return 3;
                default: return 0;
            }
        }

        public byte[] DataToSend()
        {
            ReadytoSendData.AddRange(BitConverter.GetBytes(data));
            return ReadytoSendData;
        }


    }
}

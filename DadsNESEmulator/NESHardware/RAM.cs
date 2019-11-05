/**
 *  @file           RAM.cs
 *  @brief          Defines the RAM.
 *  
 *  @copyright      2019
 *  @date           11/05/2019
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
namespace DadsNESEmulator.NESHardware
{
    public class RAM
    {
        private byte[] _ram { get; set; }

        public RAM(int size)
        {
            _ram = new byte[size];
        }
    }
}
/**
 *  @file           DefaultExpansionDevice.cs
 *  @brief          Defines default expansion device types.
 *  
 *  @copyright      2020
 *  @date           1/08/2020
 *
 *  @remark Author  Shawn M. Crawford
 *
 *  @note           N/A
 *
 */
namespace DadsNESEmulator.Types
{
    /** - https://wiki.nesdev.com/w/index.php/NES_2.0 */
    public enum DefaultExpansionDevice : byte
    {
        Unspecified = 0x00,
        StandardNESFamicomControllers = 0x01,
        NESFourScoreSatelliteWithTwoAdditionalStandardControllers = 0x02,
        FamicomFourPlayersAdapterWithTwoAdditionalStandardControllers = 0x03,
        VsSystem = 0x04,
        VsSystemWithReversedInputs = 0x05,
        VsPinballJapan = 0x06,
        VsZapper = 0x07,
        Zapper = 0x08,
        TwoZappers = 0x09,
        BandaiHyperShot = 0x0A,
        PowerPadSideA = 0x0B,
        PowerPadSideB = 0x0C,
        FamilyTrainerSideA = 0x0D,
        FamilyTrainerSideB = 0x0E,
        ArkanoidVausControllerNES = 0x0F,
        ArkanoidVausControllerFamicom = 0x10,
        TwoVausControllersPlusFamicomDataRecorder = 0x11,
        KonamiHyperShot = 0x12,
        CoconutsPachinkoController = 0x13,
        ExcitingBoxingPunchingBag = 0x14,
        JissenMahjongController = 0x15,
        PartyTap = 0x16,
        OekaKidsTablet = 0x17,
        SunsoftBarcodeBattler = 0x18,
        MiraclePianoKeyboard = 0x19,
        PokkunMoguraa = 0x1A,
        TopRider = 0x1B,
        DoubleFisted = 0x1C,
        Famicom3DSystem = 0x1D,
        DoremikkoKeyboard = 0x1E,
        ROBGyroSet = 0x1F,
        FamicomDataRecorder = 0x20,
        ASCIITurboFile = 0x21,
        IGSStorageBattleBox = 0x22,
        FamilyBASICKeyboardPlusFamicomDataRecorder = 0x23,
        DongdaPEC586Keyboard = 0x24,
        BitCorpBit79Keyboard = 0x25,
        SuborKeyboard = 0x26,
        SuborKeyboardPlusMouse3x8bitProtocol = 0x27,
        SuborKeyboardPlusMouse24bitProtocol = 0x28,
        SNESMouse = 0x29,
        Multicart = 0x2A,
        TwoSNESControllersReplacingTheTwoStandardNESControllers = 0x2B,
        RacerMateBicycle = 0x2C,
        UForce = 0x2D,
        ROBStackUp = 0x2E
    }
}
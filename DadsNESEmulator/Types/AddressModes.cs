namespace DadsNESEmulator.Types
{
    public enum AddressModes
    {
        Accumulator,
        Implied, //Implicit
        Immediate,
        ZeroPage,
        ZeroPageX,
        ZeroPageY,
        Absolute,
        AbsoluteX,
        AbsoluteY,
        Indirect,
        IndirectX,
        IndirectY,
        Relative

    }
}
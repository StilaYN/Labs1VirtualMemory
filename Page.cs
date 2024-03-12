using System.Collections;

namespace Labs1VirtualMemory;

public class Page
{
    public bool WasModified => _wasModified;

    public DateTime LastUsedTime => _lastUsedTime;

    public byte PageNumber => _pageNumber;

    public BitArray BitMap => _bitMap;

    public int[] Elements => _elements;

    private byte _pageNumber;
    private BitArray _bitMap;
    private int[] _elements;
    private bool _wasModified;
    private DateTime _lastUsedTime;

    public Page(byte pageNumber, byte[] bitMap, int[] elements)
    {
        _pageNumber = pageNumber;
        _bitMap = new BitArray(bitMap);
        _elements = elements;
        _wasModified = false;
        _lastUsedTime = DateTime.Now;
    }

    public int this[int index]
    {
        get
        {
            if (_bitMap[index])
            {
                _lastUsedTime = DateTime.Now;
                return _elements[index];
            }
            else throw new ArgumentException("Element with this index not found");
        }
        set
        {
            _wasModified = true;
            _lastUsedTime = DateTime.Now;
            _bitMap[index] = true;
            _elements[index] = value;
        }
    }
}
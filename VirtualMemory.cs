using System.Collections;
using System.IO;
using System.Text;
using File = System.IO.File;

namespace Labs1VirtualMemory;

public class VirtualMemory:IDisposable
{
    private long _length;
    private Page[] _pagesInRam;
    private String _path;
    private FileStream _fileStream;
    private const int _pageSize = 512;
    private const int _bitMapSize = 512 / sizeof(int)/8;

    public VirtualMemory(long length, string path)
    {
        _length = length;
        _path = path;
        _fileStream = CreateFile(path);
        _pagesInRam = new Page[3];
        InitializePageInRam();
    }

    public int this[long index]
    {
        get => Read(index);
        set => Write(index, value);
    }

    private FileStream CreateFile(string path)
    {
        FileStream fs;
        if (File.Exists(_path))
        {
            UTF8Encoding encoding = new UTF8Encoding();
            fs = File.Open(_path, FileMode.Open);
            byte[] firstElement = new byte[2];
            fs.Read(firstElement,0,2);
            if (encoding.GetBytes("VM").Equals(firstElement)) throw new FileLoadException("Wrong File");
        }
        else
        {
            UTF8Encoding encoding = new UTF8Encoding();
            fs = File.Create(path);
            fs.Write(encoding.GetBytes("VM"));
            long pageNumber;
            pageNumber =(_length % (_pageSize / sizeof(int)) == 0)? _length / (_pageSize / sizeof(int)): _length / (_pageSize / sizeof(int)) + 1;
            int[] a = new int[_pageSize/sizeof(int)];

            byte[] result = new byte[_pageSize];
            byte[] result1 = new byte[_bitMapSize];

            Buffer.BlockCopy(a,0,result,0,result.Length);
            for (int i = 0; i < pageNumber; i++)
            {
                fs.Write(result1,0, result1.Length);
                fs.Write(result, 0, result.Length);
            }
        }
        return fs;
    }

    private void InitializePageInRam()
    {
        for (int i = 0; i < _pagesInRam.Length; i++)
        {
            _pagesInRam[i] = ReadPageFromFile((byte)i);
        }
    }

    private void WritePageToFile(Page page)
    {
        _fileStream.Seek(2+(_pageSize+_bitMapSize)*page.PageNumber, SeekOrigin.Begin);
        byte[] bitMapInByte = new byte[_bitMapSize];
        byte[] numbersInByte = new byte[_pageSize];
        page.BitMap.CopyTo(bitMapInByte,0);
        Buffer.BlockCopy(page.Elements,0,numbersInByte,0,numbersInByte.Length);
        _fileStream.Write(bitMapInByte,0,bitMapInByte.Length);
        _fileStream.Write(numbersInByte,0,numbersInByte.Length);
    }

    private Page ReadPageFromFile(byte pageNumber)
    { 
        _fileStream.Seek(2 + (_pageSize + _bitMapSize) * pageNumber, SeekOrigin.Begin);
        byte[] bitMapInByte = new byte[_bitMapSize];
        byte[] numbersInByte = new byte[_pageSize];
        _fileStream.Read(bitMapInByte, 0, bitMapInByte.Length);
        _fileStream.Read(numbersInByte, 0, numbersInByte.Length);
        int[] elements = new int[_pageSize/sizeof(int)];
        Buffer.BlockCopy(numbersInByte,0,elements,0,numbersInByte.Length);
        return new Page(pageNumber, bitMapInByte,elements);
    }

    private int Read(long index)
    {
        return GetCurrentPages(index)[GetRelativeIndex(index)];
    }

    private void Write(long index, int value)
    {
        var page = GetCurrentPages(index);
        page[GetRelativeIndex(index)] = value;
    }

    private int GetIndexTheOldestPage()
    {
        int indexTheOldestPage = -1;
        TimeSpan maxTime = TimeSpan.Zero;
        var currentTime = DateTime.Now;

        for (int i = 0; i < _pagesInRam.Length; i++)
        {
            if (currentTime - _pagesInRam[i].LastUsedTime > maxTime)
            {
                maxTime = currentTime - _pagesInRam[i].LastUsedTime;
                indexTheOldestPage = i;
            }
        }

        return indexTheOldestPage;
    }

    private Page GetCurrentPages(long index)
    {
        byte absolutePageNumber = FindAbsolutePageNumber(index);
        for (var i = 0;i<_pagesInRam.Length;i++)
        {
            if (_pagesInRam[i].PageNumber == absolutePageNumber) return _pagesInRam[i];
        }

        return _pagesInRam[PushNewPagesInRam(absolutePageNumber)];

    }

    private int PushNewPagesInRam(byte absolutePageNumber)
    {
        int indexTheOldestPage = GetIndexTheOldestPage();
        if (_pagesInRam[indexTheOldestPage].WasModified)
        {
            WritePageToFile(_pagesInRam[indexTheOldestPage]);
            _pagesInRam[indexTheOldestPage] = ReadPageFromFile(absolutePageNumber);
        }
        else
        {
            _pagesInRam[indexTheOldestPage] = ReadPageFromFile(absolutePageNumber);
        }

        return indexTheOldestPage;

    } 

    private int GetRelativeIndex(long index)
    {
        return (int)(index - FindAbsolutePageNumber(index)*(_pageSize/sizeof(int)));
    }

    private byte FindAbsolutePageNumber(long index)
    {
        if(index>=_length||index<0) throw new ArgumentOutOfRangeException("index out  of range");
        return (byte)(index / (_pageSize / sizeof(int)));
    }

    public void Dispose()
    {
        foreach (var i in _pagesInRam)
        {
            WritePageToFile(i);
        }
        _fileStream.Close();
    }
}
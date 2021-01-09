public enum BookType
{
    Culture,
    Science,
    Education,
    Sports
}

[System.Serializable]
public class Book
{
    private string guid;           //索引
    private string name;        //书名
    private decimal price;      //价格
    private string press;       //出版社
    private int classify;
    private bool isEBook;

    public string Guid { get => guid; set => guid = value; }
    public string Name { get => name; set => name = value; }
    public decimal Price { get => price; set => price = value; }
    public string Press { get => press; set => press = value; }
    public int Classify { get => classify; set => classify = value; }
    public bool IsEBook { get => isEBook; set => isEBook = value; }
}

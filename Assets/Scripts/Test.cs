using UnityEngine;

public class Test : MonoBehaviour
{

    private void Start()
    {
        Book book = new Book();
        //Book book = new Book() { Guid = "ISBN9787111075752", Name = "设计模式-可复用面向对象软件的基础", Price = 35.89m, Press = "机械工业出版社", Classify = new int[] { 1, 2, 3 }, IsEBook = false };
        Debug.Log(SqlUtility.CreateTableText<Book>("data"));
        Debug.Log(SqlUtility.InsertTableText<Book>("data", book));
        Debug.Log(SqlUtility.SelectTableText("data", 0));
        Debug.Log(SqlUtility.UpdateTableText("data", book, 0));
        SqlUtility.Save("data", book);
        SqlUtility.Load("data", book, 5);
        SqlUtility.PrintT(book);
        book.Price = 99.9m;
        SqlUtility.Update("data", book, 5);
        SqlUtility.Load("data", book, 5);
        SqlUtility.PrintT(book);
    }
}

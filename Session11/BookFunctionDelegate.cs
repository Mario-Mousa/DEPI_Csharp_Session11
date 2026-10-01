namespace Session11
{
    // User-defined delegate with the SAME signature as GetTitle / GetAuthors / GetPrice:
    // takes a Book and returns a string
    public delegate string BookFunctionDelegate(Book B);
}
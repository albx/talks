using System;

Console.WriteLine("Ciao Twitch!");

var m1 = new Money(10, "EUR");
var m2 = new Money(10, "EUR");

Person person = new() { FirstName = "Alberto" };

person.SetMoney(new(10, "EUR"));

Console.WriteLine(m1 == m2);

if (person is not null)
{

}

public record Money(decimal Amount, string Currency);

public class Person
{
    public string FirstName { get; init; }

    public string LastName { get; }

    public Person()
    {
        LastName = "prova";
    }

    public void SetMoney(Money money)
    {
        //....
    }
}
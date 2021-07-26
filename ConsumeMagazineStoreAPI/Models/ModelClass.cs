using System;
using System.Collections.Generic;

namespace ConsumeMagazineStoreAPI.Models
{
    public class MyToken
    {
        public string Token { get; set; }
    }
    public class Category
    {
        public List<string> Data { get; set; }
    }

    public class MagazineList
    {
        public List<Magazine> Data { get; set; }
    }
    public class Magazine
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
    }

    public class SubscriberList
    {
        public List<Subscriber> Data { get; set; }
    }

    public class Subscriber
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public List<int> MagazineIds { get; set; }
    }

    public class AnswerList
    {
        public Answer Data { get; set; }
        public bool Success { get; set; }
        public string Token { get; set; }
        public string Message { get; set; }
    }

    public class Answer
    {
        public TimeSpan TotalTime { get; set; }
        public bool AnswerCorrect { get; set; }
        public string ShouldBe { get; set; }

    }

    public class PostList
    {
        public List<string> Subscribers { get; set; }
    }
}

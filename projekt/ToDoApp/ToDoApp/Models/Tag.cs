﻿namespace ToDoApp.Models
{
    /// <summary>
    ///   Model of Tag
    /// </summary>
    public class Tag
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
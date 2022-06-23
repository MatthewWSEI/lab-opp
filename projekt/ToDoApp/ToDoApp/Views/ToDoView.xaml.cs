﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using ToDoApp.Models;

namespace ToDoApp.Views
{
    /// <summary>
    /// Interaction logic for ToDoView.xaml
    /// </summary>
    public partial class ToDoView : Window
    {
        private AppDBContext context;
        public ToDoView()
        {
            InitializeComponent();

            using(context = new AppDBContext())
            {
                var statuses = context.Statuses.ToList();
                foreach(Status status in statuses)
                {
                    TaskStatus.Items.Add(status);
                }
            }

            this.RefreshData();
        }

        private void RefreshData()
        {
            using (context = new AppDBContext())
            {
                var query = from task in context.Tasks
                            orderby task.Date
                            select new TasksListView( task.Id, task.Name, task.Status.Name, task.Date );

                TasksList.ItemsSource = query.ToList();
            }
        }

        private void SignOutButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new MainWindow();
            window.Owner = this;
        }

        private void TaskCreateButton_Click(object sender, RoutedEventArgs e)
        {
            if(TaskName.Text == null || TaskStatus.SelectedItem == null)
            {
                MessageBox.Show("Please make sure all data has been entered!");
                return;
            }

            var newTask = new Models.Task
            {
                Name = TaskName.Text,
                StatusId = (TaskStatus.SelectedItem as Status).Id,
                Date = TaskDate.SelectedDate
            };

            using (context = new AppDBContext())
            {
                context.Tasks.Add(newTask);
                context.SaveChanges();
            }

            this.RefreshData();
        }

        private void TaskDeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksList.SelectedItem == null)
            {
                MessageBox.Show("Please select any record!");
                return;
            }

            using (context = new AppDBContext())
            {
                var selectedItem = (TasksListView)TasksList.SelectedItem;

                var item = context.Tasks.Find(selectedItem.Id);

                context.Tasks.Remove(item);
                context.SaveChanges();
            }

            this.RefreshData();

        }

        private void TaskUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (TasksList.SelectedItem == null)
            {
                MessageBox.Show("Please select any record!");
                return;
            }

            using (context = new AppDBContext())
            {
                var selectedItem = (TasksListView)TasksList.SelectedItem;

                var task = context.Tasks.Find(selectedItem.Id);

                task.Name = TaskName.Text;
                task.StatusId = (TaskStatus.SelectedItem as Status).Id;
                task.Date = TaskDate.SelectedDate;

                context.Tasks.Update(task);
                context.SaveChanges();
            }

            this.RefreshData();

        }

        private void TasksList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TasksList.SelectedItem == null)
                return;

            var selectedItem = (TasksListView)TasksList.SelectedItem;
            TaskName.Text = selectedItem.Name;

            using (context = new AppDBContext())
            {
                var task = context.Tasks.Find(selectedItem.Id);
                var newStatus = context.Statuses.Find(task.StatusId).Id - 1;
                TaskStatus.SelectedIndex = newStatus;
            }

            TaskDate.SelectedDate = selectedItem.Date;
        }
    }

    public class TasksListView
    {
        // task.Id, task.Name, StatusName = task.Status.Name, task.Date
        public int Id { get; set; }
        public string Name { get; set; }
        public string StatusName { get; set; }
        public DateTime? Date { get; set; }

        public TasksListView(int id, string name, string statusName, DateTime? date)
        {
            this.Id = id;
            this.Name = name;
            this.StatusName = statusName;
            this.Date = date;
        }
    }
}

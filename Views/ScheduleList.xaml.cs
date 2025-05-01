using System;
using System.Linq;
using Microsoft.Maui.Controls;
using SmartHomeApp.Models;
using SmartHomeApp.Services;
using SmartHomeApp.ViewModels;
using static SmartHomeApp.AppShell;

namespace SmartHomeApp.Views
{
    public partial class ScheduleListPage : ContentPage
    {
        MainViewModel Vm => AppViewModel;

        public ScheduleListPage()
        {
            InitializeComponent();
            BindingContext = Vm;
        }

        // Navigate to the creation/edit page
        private async void OnNewClicked(object sender, EventArgs e)
            => await Navigation.PushAsync(new SchedulePage());

        // Edit existing schedule
        private async void OnEditClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            // Preselect and navigate to SchedulePage
            var sched = Vm.Schedules.FirstOrDefault(s => s.Id == id);
            if (sched == null) return;

            var page = new SchedulePage();
            // OnAppearing or constructor of SchedulePage could detect
            // a static “CurrentEditing” in VM, but simplest:
            // pass via AppShell.AppViewModel a property, or
            // add an overload on SchedulePage to accept a schedule.
            // For now, we’ll just reuse inline editing.
            MessagingCenter.Send(this, "EditSchedule", id);
            await Navigation.PushAsync(page);
        }

        // Delete schedule
        private async void OnDeleteClicked(object sender, EventArgs e)
        {
            if ((sender as Button)?.CommandParameter is not int id) return;
            var sched = Vm.Schedules.FirstOrDefault(s => s.Id == id);
            if (sched == null) return;
            await DatabaseService.DeleteSchedule(sched);
            Vm.Schedules.Remove(sched);
        }

        // Optional: persist reorder
        private void OnReorderCompleted(object sender, EventArgs e)
        {
            // no-op
        }
    }
}

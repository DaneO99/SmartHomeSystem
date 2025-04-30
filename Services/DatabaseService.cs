using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;
using SmartHomeApp.Models;
using DeviceModel = SmartHomeApp.Models.Device;

namespace SmartHomeApp.Services
{
    public static class DatabaseService
    {
        private static SQLiteAsyncConnection? _database;

        public static async Task InitializeAsync()
        {
            if (_database != null)
                return;

            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "smarthome.db3");

            _database = new SQLiteAsyncConnection(dbPath);

            await _database.CreateTableAsync<DeviceGroup>();
            await _database.CreateTableAsync<DeviceModel>();
            await _database.CreateTableAsync<DeviceSchedule>();
        }

        // Retrieval
        public static Task<List<DeviceGroup>> GetDeviceGroups() =>
            _database!.Table<DeviceGroup>().ToListAsync();

        public static Task<List<DeviceModel>> GetDevices() =>
            _database!.Table<DeviceModel>().ToListAsync();

        public static Task<List<DeviceSchedule>> GetSchedules() =>
            _database!.Table<DeviceSchedule>().ToListAsync();

        // Save or update
        public static async Task<int> SaveGroup(DeviceGroup group)
        {
            if (group.Id != 0)
                return await _database!.UpdateAsync(group);
            return await _database!.InsertAsync(group);
        }

        public static async Task<int> SaveDevice(DeviceModel device)
        {
            if (device.Id != 0)
                return await _database!.UpdateAsync(device);
            return await _database!.InsertAsync(device);
        }

        public static async Task<int> SaveSchedule(DeviceSchedule schedule)
        {
            if (schedule.Id != 0)
                return await _database!.UpdateAsync(schedule);
            return await _database!.InsertAsync(schedule);
        }

        // Deletion
        public static Task<int> DeleteGroup(DeviceGroup group) =>
            _database!.DeleteAsync(group);

        public static Task<int> DeleteDevice(DeviceModel device) =>
            _database!.DeleteAsync(device);

        public static Task<int> DeleteSchedule(DeviceSchedule schedule) =>
            _database!.DeleteAsync(schedule);
    }
}

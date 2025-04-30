// Dependency imports for file I/O, data collections, async operations, and SQLite
using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using SQLite;                             // Provides async SQLite connection and table management
using SmartHomeApp.Models;
using DeviceModel = SmartHomeApp.Models.Device;    // Alias to distinguish Device model type

namespace SmartHomeApp.Services
{
    /// <summary>
    /// Provides initialization and CRUD (create, read, update, delete) operations
    /// for application data stored in a local SQLite database.
    /// </summary>
    public static class DatabaseService
    {
        // Underlying async connection to SQLite database; initialized on first use
        private static SQLiteAsyncConnection? _database;

        /// <summary>
        /// Initializes the SQLite database connection and creates required tables if they do not exist.
        /// Safe to call multiple times; subsequent calls have no effect.
        /// </summary>
        public static async Task InitializeAsync()
        {
            // Skip initialization if connection already established
            if (_database != null)
                return;

            // Determine file path in local application data directory
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "smarthome.db3");

            // Instantiate asynchronous connection to SQLite database
            _database = new SQLiteAsyncConnection(dbPath);

            // Create tables for group, device, and schedule models
            await _database.CreateTableAsync<DeviceGroup>();
            await _database.CreateTableAsync<DeviceModel>();
            await _database.CreateTableAsync<DeviceSchedule>();
        }

        // -------------------- Retrieval Methods --------------------

        /// <summary>
        /// Retrieves all device groups stored in the database.
        /// </summary>
        /// <returns>List of persisted <see cref="DeviceGroup"/> entries.</returns>
        public static Task<List<DeviceGroup>> GetDeviceGroups() =>
            _database!.Table<DeviceGroup>().ToListAsync();

        /// <summary>
        /// Retrieves all devices stored in the database.
        /// </summary>
        /// <returns>List of persisted <see cref="DeviceModel"/> entries.</returns>
        public static Task<List<DeviceModel>> GetDevices() =>
            _database!.Table<DeviceModel>().ToListAsync();

        /// <summary>
        /// Retrieves all device schedules stored in the database.
        /// </summary>
        /// <returns>List of persisted <see cref="DeviceSchedule"/> entries.</returns>
        public static Task<List<DeviceSchedule>> GetSchedules() =>
            _database!.Table<DeviceSchedule>().ToListAsync();

        // -------------------- Save or Update Methods --------------------

        /// <summary>
        /// Inserts a new group or updates an existing one in the database.
        /// </summary>
        /// <param name="group">Instance of <see cref="DeviceGroup"/> to save.</param>
        /// <returns>Number of rows affected by the operation.</returns>
        public static async Task<int> SaveGroup(DeviceGroup group)
        {
            // Perform update when primary key is set; otherwise insert
            if (group.Id != 0)
                return await _database!.UpdateAsync(group);
            return await _database!.InsertAsync(group);
        }

        /// <summary>
        /// Inserts a new device or updates an existing one in the database.
        /// </summary>
        /// <param name="device">Instance of <see cref="DeviceModel"/> to save.</param>
        /// <returns>Number of rows affected by the operation.</returns>
        public static async Task<int> SaveDevice(DeviceModel device)
        {
            if (device.Id != 0)
                return await _database!.UpdateAsync(device);
            return await _database!.InsertAsync(device);
        }

        /// <summary>
        /// Inserts a new schedule or updates an existing one in the database.
        /// </summary>
        /// <param name="schedule">Instance of <see cref="DeviceSchedule"/> to save.</param>
        /// <returns>Number of rows affected by the operation.</returns>
        public static async Task<int> SaveSchedule(DeviceSchedule schedule)
        {
            if (schedule.Id != 0)
                return await _database!.UpdateAsync(schedule);
            return await _database!.InsertAsync(schedule);
        }

        // -------------------- Deletion Methods --------------------

        /// <summary>
        /// Deletes the specified device group from the database.
        /// </summary>
        /// <param name="group">Instance of <see cref="DeviceGroup"/> to delete.</param>
        /// <returns>Number of rows deleted.</returns>
        public static Task<int> DeleteGroup(DeviceGroup group) =>
            _database!.DeleteAsync(group);

        /// <summary>
        /// Deletes the specified device from the database.
        /// </summary>
        /// <param name="device">Instance of <see cref="DeviceModel"/> to delete.</param>
        /// <returns>Number of rows deleted.</returns>
        public static Task<int> DeleteDevice(DeviceModel device) =>
            _database!.DeleteAsync(device);

        /// <summary>
        /// Deletes the specified schedule from the database.
        /// </summary>
        /// <param name="schedule">Instance of <see cref="DeviceSchedule"/> to delete.</param>
        /// <returns>Number of rows deleted.</returns>
        public static Task<int> DeleteSchedule(DeviceSchedule schedule) =>
            _database!.DeleteAsync(schedule);
    }
}

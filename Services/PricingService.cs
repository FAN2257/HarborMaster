using HarborMaster.Models;
using HarborMaster.Repositories;
using System;
using System.Threading.Tasks;

namespace HarborMaster.Services
{
    public class PricingService
    {
        private readonly ShipTypeMultiplierRepository _shipTypeMultiplierRepo;
        private readonly SizeMultiplierRepository _sizeMultiplierRepo;

        public PricingService()
        {
            _shipTypeMultiplierRepo = new ShipTypeMultiplierRepository();
            _sizeMultiplierRepo = new SizeMultiplierRepository();
        }

        /// <summary>
        /// Calculate berth cost based on actual arrival/departure time
        /// Formula: (base_rate × actual_days × size_multiplier × type_multiplier) + special_fee
        /// Uses POLYMORPHISM: ship.CalculateSpecialFee() can be overridden by derived classes
        /// </summary>
        /// <param name="berth">The berth being used</param>
        /// <param name="ship">The ship information</param>
        /// <param name="actualArrival">Actual arrival time</param>
        /// <param name="actualDeparture">Actual departure time</param>
        /// <returns>Total cost in decimal</returns>
        public async Task<decimal> CalculateBerthCost(
            Berth berth,
            Ship ship,
            DateTime actualArrival,
            DateTime actualDeparture)
        {
            // 1. Calculate actual duration (rounded up to days)
            TimeSpan duration = actualDeparture - actualArrival;
            int actualDays = (int)Math.Ceiling(duration.TotalDays);

            // Minimum 1 day charge
            if (actualDays < 1)
                actualDays = 1;

            // 2. Get base rate from berth
            decimal baseRate = berth.BaseRatePerDay;

            // 3. Get size multiplier based on ship length
            decimal sizeMultiplier = await _sizeMultiplierRepo.GetMultiplierValue((decimal)ship.LengthOverall);

            // 4. Get type multiplier based on ship type
            decimal typeMultiplier = await _shipTypeMultiplierRepo.GetMultiplierValue(ship.ShipType);

            // 5. Calculate base docking cost
            decimal dockingCost = baseRate * actualDays * sizeMultiplier * typeMultiplier;

            // 6. Add special fee using POLYMORPHISM
            // This calls the virtual method from Ship class
            // If ship is a derived class, it will use the overridden method
            decimal specialFee = ship.CalculateSpecialFee();

            // 7. Total cost
            decimal totalCost = dockingCost + specialFee;

            return totalCost;
        }

        /// <summary>
        /// Calculate estimated cost based on planned duration (for preview before arrival)
        /// Uses POLYMORPHISM: ship.CalculateSpecialFee()
        /// </summary>
        public async Task<decimal> CalculateEstimatedCost(
            Berth berth,
            Ship ship,
            int plannedDurationDays)
        {
            decimal baseRate = berth.BaseRatePerDay;
            decimal sizeMultiplier = await _sizeMultiplierRepo.GetMultiplierValue((decimal)ship.LengthOverall);
            decimal typeMultiplier = await _shipTypeMultiplierRepo.GetMultiplierValue(ship.ShipType);

            decimal dockingCost = baseRate * plannedDurationDays * sizeMultiplier * typeMultiplier;

            // Add special fee using POLYMORPHISM
            decimal specialFee = ship.CalculateSpecialFee();

            decimal estimatedCost = dockingCost + specialFee;

            return estimatedCost;
        }

        /// <summary>
        /// Get breakdown of cost calculation for display/debugging
        /// Shows how POLYMORPHISM is used in pricing
        /// </summary>
        public async Task<PricingBreakdown> GetPricingBreakdown(
            Berth berth,
            Ship ship,
            DateTime actualArrival,
            DateTime actualDeparture)
        {
            TimeSpan duration = actualDeparture - actualArrival;
            int actualDays = (int)Math.Ceiling(duration.TotalDays);
            if (actualDays < 1) actualDays = 1;

            decimal baseRate = berth.BaseRatePerDay;
            decimal sizeMultiplier = await _sizeMultiplierRepo.GetMultiplierValue((decimal)ship.LengthOverall);
            decimal typeMultiplier = await _shipTypeMultiplierRepo.GetMultiplierValue(ship.ShipType);
            decimal dockingCost = baseRate * actualDays * sizeMultiplier * typeMultiplier;

            // POLYMORPHISM: Call virtual method
            decimal specialFee = ship.CalculateSpecialFee();

            decimal totalCost = dockingCost + specialFee;

            return new PricingBreakdown
            {
                BaseRatePerDay = baseRate,
                ActualDays = actualDays,
                SizeMultiplier = sizeMultiplier,
                TypeMultiplier = typeMultiplier,
                SpecialFee = specialFee,
                DockingCost = dockingCost,
                TotalCost = totalCost
            };
        }
    }

    /// <summary>
    /// Helper class to show pricing breakdown
    /// Demonstrates how POLYMORPHISM affects pricing
    /// </summary>
    public class PricingBreakdown
    {
        public decimal BaseRatePerDay { get; set; }
        public int ActualDays { get; set; }
        public decimal SizeMultiplier { get; set; }
        public decimal TypeMultiplier { get; set; }
        public decimal DockingCost { get; set; }
        public decimal SpecialFee { get; set; }
        public decimal TotalCost { get; set; }

        public override string ToString()
        {
            return $"Docking: Rp {BaseRatePerDay:N0} × {ActualDays} hari × {SizeMultiplier} × {TypeMultiplier} = Rp {DockingCost:N0}\n" +
                   $"Special Fee: Rp {SpecialFee:N0}\n" +
                   $"Total: Rp {TotalCost:N0}";
        }
    }
}

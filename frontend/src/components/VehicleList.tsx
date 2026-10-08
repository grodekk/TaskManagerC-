import type { Vehicle } from '../types/vehicle'

type VehicleListProps = {
  vehicles: Vehicle[]
  onVehicleClick: (vehicleId: number) => void
}

function VehicleList({
  vehicles,
  onVehicleClick
}: VehicleListProps) {
  return (
    <div>
      <h2>Vehicles</h2>

      {vehicles.map((vehicle) => (
        <div key={vehicle.id}>
          <button onClick={() => onVehicleClick(vehicle.id)}>
            {vehicle.registrationNumber}
          </button>
        </div>
      ))}
    </div>
  )
}

export default VehicleList
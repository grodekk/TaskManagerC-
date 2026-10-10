import type { Vehicle } from '../types/vehicle'

const apiUrl = import.meta.env.VITE_API_URL

export async function getVehicles(token: string): Promise<Vehicle[]> {
  const response = await fetch(`${apiUrl}/api/vehicles`, {
    headers: {
      Authorization: `Bearer ${token}`
    }
  })

  if (response.status === 403) {
  throw new Error('You do not have permission to view vehicle records.')
  }

  if (!response.ok) {
    throw new Error(`Failed to load vehicles: ${response.status}`)
  }

  const data: Vehicle[] = await response.json()

  return data
}
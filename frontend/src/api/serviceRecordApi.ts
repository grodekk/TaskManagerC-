import type { ServiceRecord } from '../types/ServiceRecord'

const apiUrl = import.meta.env.VITE_API_URL

export async function getServiceRecords(
  vehicleId: number,
  token: string
): Promise<ServiceRecord[]> {
  const response = await fetch(
    `${apiUrl}/api/vehicles/${vehicleId}/service-records`,
    {
      headers: {
        Authorization: `Bearer ${token}`
      }
    }
  )

  if (!response.ok) {
    throw new Error(`Failed to load service records: ${response.status}`)
  }

  const data: ServiceRecord[] = await response.json()

  return data
}
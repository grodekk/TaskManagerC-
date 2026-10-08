export type ServiceRecordItem = {
  id: number
  description: string
}

export type ServiceRecord = {
  id: number
  performedOn: string
  mileage: number
  items: ServiceRecordItem[]
}
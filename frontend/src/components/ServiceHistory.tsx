import type { ServiceRecord } from '../types/ServiceRecord'

type ServiceHistoryProps = {
  records: ServiceRecord[]
  visible: boolean
}

function ServiceHistory({
  records,
  visible
}: ServiceHistoryProps) {
  if (!visible) {
    return null
  }

  return (
    <div>
      <h2>Service History</h2>

      {records.map((record) => (
        <div key={record.id}>
          <p>
            {record.performedOn} - {record.mileage} km
          </p>

          {record.items.map((item) => (
            <div key={item.id}>
              {item.description}
            </div>
          ))}
        </div>
      ))}
    </div>
  )
}

export default ServiceHistory
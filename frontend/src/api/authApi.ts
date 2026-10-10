type LoginResponse = {
  token: string
}

type ApiErrorResponse = {
  code: string
  message: string
}

const apiUrl = import.meta.env.VITE_API_URL

export async function login(
  username: string,
  password: string
): Promise<LoginResponse> {
  const response = await fetch(`${apiUrl}/api/auth/login`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username,
      password
    })
  })

  if (!response.ok) {
    const error: ApiErrorResponse = await response.json()

    throw new Error(error.message)
  }

  const data: LoginResponse = await response.json()

  return data
}


export async function register(
  username: string,
  password: string
): Promise<void> {
  const response = await fetch(`${apiUrl}/api/auth/register`, {
    method: 'POST',
    headers: {
      'Content-Type': 'application/json'
    },
    body: JSON.stringify({
      username,
      password
    })
  })

  if (!response.ok) {
    const error: ApiErrorResponse = await response.json()

    throw new Error(error.message)
  }
}
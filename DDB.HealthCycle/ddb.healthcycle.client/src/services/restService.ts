import axios, { AxiosRequestConfig } from "axios";

export type csAxiosRequest<T> = {
  uri: string;
  body?: object; // lets make a type union here instead when we start adding types with calls?
  onSuccess?: (data: T) => void;
};

export const headerConfig: AxiosRequestConfig = {
  headers: {
    'Content-Type': 'application/json'
  }
};


export const GetFromApi = async <T>({ uri, onSuccess }: csAxiosRequest<T>) : Promise<boolean> => {
  const result = await axios.get(uri);
  const wasSuccess = result.status === 200
  if (wasSuccess) {
    onSuccess?.(result.data as T);
  }
  // TODO: add error handling for common errors: 204, 400 and 500 
  return wasSuccess;
}

const RestService = {
  get: GetFromApi,
};

export default RestService;

import useAuthStore from '@store/AuthStore.ts';

import { EmailTamerApiContext } from './emailTamerApiContext';

const baseUrl = import.meta.env.VITE_API_URL;

export type ErrorWrapper<TError> =
  | TError
  | { status: number; payload: string };

export type EmailTamerApiFetcherOptions<
  TBody,
  THeaders,
  TQueryParams,
  TPathParams,
> = {
  url: string;
  method: string;
  body?: TBody;
  headers?: THeaders;
  queryParams?: TQueryParams;
  pathParams?: TPathParams;
  signal?: AbortSignal;
} & EmailTamerApiContext['fetcherOptions'];

export async function emailTamerApiFetch<
  TData,
  TError,
  TBody extends {} | FormData | undefined | null,
  THeaders extends {},
  TQueryParams extends {},
  TPathParams extends {},
>({
    url,
    method,
    body,
    headers,
    pathParams,
    queryParams,
    signal,
}: EmailTamerApiFetcherOptions<
  TBody,
  THeaders,
  TQueryParams,
  TPathParams
>): Promise<TData> {
    let error: ErrorWrapper<TError>;
    try {
        const token = useAuthStore.getState().token;
        const authHeader = token ? { ['Authorization']: `Bearer ${token}`} : null;
        const requestHeaders: HeadersInit = {
            'Content-Type': 'application/json',
            ...authHeader,
            ...headers,
        };

        /**
     * As the fetch API is being used, when multipart/form-data is specified
     * the Content-Type header must be deleted so that the browser can set
     * the correct boundary.
     * https://developer.mozilla.org/en-US/docs/Web/API/FormData/Using_FormData_Objects#sending_files_using_a_formdata_object
     */
        if (
            requestHeaders['Content-Type']
                ?.toLowerCase()
                .includes('multipart/form-data')
        ) {
            delete requestHeaders['Content-Type'];
        }

        const response = await window.fetch(
            `${baseUrl}${resolveUrl(url, queryParams, pathParams)}`,
            {
                signal,
                method: method.toUpperCase(),
                body: body
                    ? body instanceof FormData
                        ? body
                        : JSON.stringify(body)
                    : undefined,
                headers: requestHeaders,
            },
        );
        if (!response.ok) {
            try {
                error = await response.json();
            } catch (e) {
                error = {
                    status: response.status,
                    payload:
            e instanceof Error
                ? `Unexpected error (${e.message})`
                : 'Unexpected error',
                };
            }
        } else if (response.headers.get('content-type')?.includes('json')) {
            return await response.json();
        } else {
            // if it is not a json response, assume it is a blob and cast it to TData
            return (await response.blob()) as unknown as TData;
        }
    } catch (e) {
        const errorObject: Error = {
            name: 'unknown' as const,
            message:
        e instanceof Error ? `Network error (${e.message})` : 'Network error',
            stack: e as string,
        };
        throw errorObject;
    }
    throw error;
}

const resolveUrl = (
    url: string,
    queryParams: Record<string, string> = {},
    pathParams: Record<string, string> = {},
) => {
    let query = new URLSearchParams(queryParams).toString();
    if (query) query = `?${query}`;
    return (
        url.replace(/\{\w*\}/g, (key) => pathParams[key.slice(1, -1)] ?? '') + query
    );
};

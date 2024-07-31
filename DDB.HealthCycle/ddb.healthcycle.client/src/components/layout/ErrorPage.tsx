import { useRouteError } from "react-router-dom";

type errorResponse = {
  statusText?: string;
  message?: string;
}

const ErrorPage: React.FC  = () => {
    const error: unknown = useRouteError();
    console.error(error);

  const errObj = error as errorResponse;

    return (
    <div id="error-page">
        <h1>Oops!</h1>
        <p>Sorry, an unexpected error has occurred.</p>
        <p>
            <i>{errObj?.statusText || errObj?.message}</i>
        </p>
    </div>
    );
};

export default ErrorPage;

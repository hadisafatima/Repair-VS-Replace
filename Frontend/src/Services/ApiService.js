import API_BASE_URL from "./api";

export async function apiRequest(endpoint, options = {}) {
    const response = await fetch(`${API_BASE_URL}${endpoint}`, {
        ...options,
        headers: {
            "Content-Type": "application/json",
            ...(options.headers || {}),
        },
    });

    if (!response.ok) {
        let message = `Request failed with status ${response.status}`;

        try {
            const errorData = await response.json();

            console.log("API Error Response:", errorData);

            if (errorData?.errors) {
                console.log("Validation Errors:", errorData.errors);
            }

            if (errorData?.error) {
                message = errorData.error;
            } else if (errorData?.title) {
                message = errorData.title;
            }
        } catch {
            // Response wasn't JSON
        }

        throw new Error(message);
    }

    return await response.json();
}
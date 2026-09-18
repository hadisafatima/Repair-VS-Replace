import { apiRequest } from "./ApiService";

export async function calculateRepairReplace(requuestData) {
    return await apiRequest(
        "/Calculation",
        {
            method : "POST",
            body : JSON.stringify(requuestData),
        }
    );
}
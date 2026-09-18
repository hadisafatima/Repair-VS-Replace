import { apiRequest } from "./ApiService";

export async function getProductTypes() {
    return await apiRequest("/ProductTypes");
}

export async function getCategories(){
    return await apiRequest("/ProductTypes/categories");
}

export async function getProductTypeById(id) {
    return await apiRequest(`/ProductTypes/${id}`)
}
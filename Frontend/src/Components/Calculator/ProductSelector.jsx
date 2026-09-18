import { useEffect, useState } from "react";
import { getProductTypes } from "../../services/productService";
import { inputClass, labelClass } from "./formStyles";

function ProductSelector({ value, onChange }) {
    const [products, setProducts] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        async function loadProducts() {
            try {
                const data = await getProductTypes();
                setProducts(data);
            } catch (error) {
                setError(error.message);
            } finally {
                setLoading(false);
            }
        }

        loadProducts();
    }, []);

    return (
        <div>
            <label htmlFor="productType" className={labelClass}>
                Product
            </label>

            {loading && (
                <p className="mt-2 text-sm text-slate-500">
                    Loading products...
                </p>
            )}

            {error && (
                <p className="mt-2 text-sm text-red-600">
                    {error}
                </p>
            )}

            {!loading && !error && (
                <select
                    id="productType"
                    value={value}
                    onChange={(e) => onChange(e.target.value)}
                    className={inputClass}
                >
                    <option value="">
                        Select a product
                    </option>

                    {products.map((product) => (
                        <option
                            key={product.id}
                            value={product.id}
                        >
                            {product.displayName}
                        </option>
                    ))}
                </select>
            )}
        </div>
    );
}

export default ProductSelector;
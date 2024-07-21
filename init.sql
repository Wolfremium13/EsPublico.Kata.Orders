CREATE TABLE orders
(
    order_id       BIGINT PRIMARY KEY,
    uuid           UUID UNIQUE,
    order_priority VARCHAR(1),
    order_date     DATE,
    region         VARCHAR(150),
    country        VARCHAR(150),
    item_type      VARCHAR(150),
    sales_channel  VARCHAR(150),
    ship_date      DATE,
    units_sold     BIGINT,
    unit_price     DECIMAL(10, 2),
    unit_cost      DECIMAL(10, 2),
    total_revenue  DECIMAL(10, 2),
    total_cost     DECIMAL(10, 2),
    total_profit   DECIMAL(10, 2),
    created_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    updated_at     TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

CREATE INDEX idx_order_id ON orders (order_id);
CREATE INDEX idx_region ON orders (region);
CREATE INDEX idx_country ON orders (country);
CREATE INDEX idx_item_type ON orders (item_type);
CREATE INDEX idx_sales_channel ON orders (sales_channel);
CREATE INDEX idx_order_priority ON orders (order_priority);
CREATE INDEX idx_summary ON orders (region, country, item_type, sales_channel, order_priority);

CREATE
    OR REPLACE FUNCTION update_updated_at_column()
    RETURNS TRIGGER AS
$$
BEGIN
    NEW.updated_at
        = NOW();
    RETURN NEW;
END;
$$
    LANGUAGE plpgsql;

CREATE TRIGGER update_orders_updated_at
    BEFORE UPDATE
    ON orders
    FOR EACH ROW
EXECUTE FUNCTION update_updated_at_column();

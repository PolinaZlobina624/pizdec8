-- Таблица сотрудников
CREATE TABLE employees (
                           id SERIAL PRIMARY KEY,
                           first_name VARCHAR( 50 ) NOT NULL,
                           last_name VARCHAR( 50 ) NOT NULL,
                           position VARCHAR( 20 ) CHECK ( position IN ( 'администратор' , 'официант' , 'повар' )),
                           hire_date DATE NOT NULL,
                           layoff_date DATE DEFAULT NULL,
                           password_hash TEXT NOT NULL,
                           email VARCHAR( 100 ) UNIQUE NOT NULL,
                           phone_number VARCHAR( 20 ),
                           is_active BOOLEAN DEFAULT TRUE,
                           created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                           role_id INT REFERENCES roles(id)
);
CREATE TABLE roles (
                       id SERIAL PRIMARY KEY,
                       name VARCHAR( 50 ) NOT NULL,
                       permissions JSON
);
-- Таблица столов
CREATE TABLE tables (
                        id SERIAL PRIMARY KEY,
                        table_number VARCHAR(10) UNIQUE NOT NULL,
                        capacity SMALLINT NOT NULL CHECK(capacity > 0),
                        is_available BOOLEAN NOT NULL DEFAULT TRUE,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- Таблица меню (объединенная категории и позиции)
CREATE TABLE menu_items (
                            id SERIAL PRIMARY KEY,
                            food_name VARCHAR(100) NOT NULL,
                            price DECIMAL(10, 2) NOT NULL CHECK(price > 0),
                            category VARCHAR(50) NOT NULL CHECK (category IN ('горячие блюда', 'холодные
закуски', 'напитки', 'десерты')),
                            description TEXT,
                            preparation_time INT,
                            is_available BOOLEAN DEFAULT TRUE,
                            created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- Таблица смен
CREATE TABLE shifts (
                        id SERIAL PRIMARY KEY,
                        shift_date DATE NOT NULL,
                        shift_type VARCHAR(20) NOT NULL CHECK (shift_type IN ('утренняя', 'вечерняя',
                                                                              'ночная')),
                        start_time TIME NOT NULL,
                        end_time TIME NOT NULL,
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
-- Таблица назначений сотрудников на смены
CREATE TABLE shift_assignments (
                                   id SERIAL PRIMARY KEY,
                                   shift_id INT REFERENCES shifts(id) ON DELETE CASCADE,
                                   employee_id INT REFERENCES employees(id) ON DELETE CASCADE,
                                   assigned_tables INT[],
                                   created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                   UNIQUE(shift_id, employee_id)
);
-- Таблица заказов
CREATE TABLE orders (
                        id SERIAL PRIMARY KEY,
                        table_id INT REFERENCES tables(id) ON DELETE SET NULL,
                        waiter_id INT REFERENCES employees(id) ON DELETE SET NULL,
                        shift_id INT REFERENCES shifts(id) ON DELETE SET NULL,
                        order_status VARCHAR(20) NOT NULL DEFAULT 'принят' CHECK (order_status IN
                                                                                  ('принят', 'готовится', 'готов', 'оплачен', 'отменен')),
                        payment_method VARCHAR(20) CHECK (payment_method IN ('наличные',
                                                                             'безналичные')),
                        payment_status VARCHAR(20) DEFAULT 'не оплачен' CHECK (payment_status IN ('не
оплачен', 'оплачен')),
                        total_amount DECIMAL(10, 2) NOT NULL DEFAULT 0 CHECK(total_amount >= 0),
                        created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        updated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                        completed_at TIMESTAMP NULL
);
-- Таблица связи заказов и блюд
CREATE TABLE order_menu_items (
                                  id SERIAL PRIMARY KEY,
                                  order_id INT REFERENCES orders(id) ON DELETE CASCADE,
                                  menu_item_id INT REFERENCES menu_items(id) ON DELETE CASCADE,
                                  quantity INT NOT NULL CHECK(quantity > 0),
                                  unit_price DECIMAL(10, 2) NOT NULL CHECK(unit_price >= 0),
                                  item_status VARCHAR(20) DEFAULT 'ожидает' CHECK (item_status IN ('ожидает',
                                                                                                   'готовится', 'готово', 'подано')),
                                  chef_id INT REFERENCES employees(id) ON DELETE SET NULL,
                                  notes TEXT,
                                  created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
                                  UNIQUE(order_id, menu_item_id)
);
-- Таблица финансовых отчетов
CREATE TABLE financial_reports (
                                   id SERIAL PRIMARY KEY,
                                   report_type VARCHAR(50) NOT NULL CHECK (report_type IN ('сменный', 'дневной',
                                                                                           'недельный', 'месячный')),
                                   shift_id INT REFERENCES shifts(id) ON DELETE SET NULL,
                                   generated_by INT REFERENCES employees(id) ON DELETE SET NULL,
                                   data_range_start DATE NOT NULL,
                                   data_range_end DATE NOT NULL,
                                   total_revenue DECIMAL(12, 2) NOT NULL DEFAULT 0,
                                   cash_revenue DECIMAL(12, 2) NOT NULL DEFAULT 0,
                                   card_revenue DECIMAL(12, 2) NOT NULL DEFAULT 0,
                                   total_orders INT NOT NULL DEFAULT 0,
                                   employee_ids JSONB,
                                   summary_data JSONB,
                                   generated_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);

select * from financial_reports;
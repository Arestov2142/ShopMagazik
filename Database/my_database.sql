--
-- PostgreSQL database dump
--

\restrict rlY0jTCqXdJHWwe8KwevndYy0gh4OLrT60ZgQeh3gk37fFJxWdzhSE4mL8SaCrY

-- Dumped from database version 18.3
-- Dumped by pg_dump version 18.3

-- Started on 2026-11-02 22:45:00

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 220 (class 1259 OID 36281)
-- Name: clients; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.clients (
    client_id integer NOT NULL,
    full_name character varying(150) NOT NULL,
    phone character varying(20),
    email character varying(100),
    address character varying(250)
);


ALTER TABLE public.clients OWNER TO postgres;

--
-- TOC entry 219 (class 1259 OID 36280)
-- Name: clients_client_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.clients_client_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.clients_client_id_seq OWNER TO postgres;

--
-- TOC entry 5085 (class 0 OID 0)
-- Dependencies: 219
-- Name: clients_client_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.clients_client_id_seq OWNED BY public.clients.client_id;


--
-- TOC entry 224 (class 1259 OID 36308)
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orders (
    order_id integer NOT NULL,
    client_id integer NOT NULL,
    product_id integer NOT NULL,
    quantity integer NOT NULL,
    order_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    status character varying(50) DEFAULT 'Новый'::character varying NOT NULL,
    CONSTRAINT orders_quantity_check CHECK ((quantity > 0))
);


ALTER TABLE public.orders OWNER TO postgres;

--
-- TOC entry 223 (class 1259 OID 36307)
-- Name: orders_order_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orders_order_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.orders_order_id_seq OWNER TO postgres;

--
-- TOC entry 5086 (class 0 OID 0)
-- Dependencies: 223
-- Name: orders_order_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_order_id_seq OWNED BY public.orders.order_id;


--
-- TOC entry 230 (class 1259 OID 36369)
-- Name: password_reset_requests; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.password_reset_requests (
    request_id integer NOT NULL,
    user_id integer NOT NULL,
    request_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    status character varying(30) DEFAULT 'Ожидает'::character varying NOT NULL,
    completed_date timestamp without time zone
);


ALTER TABLE public.password_reset_requests OWNER TO postgres;

--
-- TOC entry 229 (class 1259 OID 36368)
-- Name: password_reset_requests_request_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.password_reset_requests_request_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.password_reset_requests_request_id_seq OWNER TO postgres;

--
-- TOC entry 5087 (class 0 OID 0)
-- Dependencies: 229
-- Name: password_reset_requests_request_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.password_reset_requests_request_id_seq OWNED BY public.password_reset_requests.request_id;


--
-- TOC entry 222 (class 1259 OID 36294)
-- Name: products; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.products (
    product_id integer NOT NULL,
    product_name character varying(150) NOT NULL,
    price numeric(10,2) NOT NULL,
    quantity_in_stock integer DEFAULT 0 NOT NULL,
    CONSTRAINT products_price_check CHECK ((price >= (0)::numeric)),
    CONSTRAINT products_quantity_in_stock_check CHECK ((quantity_in_stock >= 0))
);


ALTER TABLE public.products OWNER TO postgres;

--
-- TOC entry 221 (class 1259 OID 36293)
-- Name: products_product_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.products_product_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.products_product_id_seq OWNER TO postgres;

--
-- TOC entry 5088 (class 0 OID 0)
-- Dependencies: 221
-- Name: products_product_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.products_product_id_seq OWNED BY public.products.product_id;


--
-- TOC entry 226 (class 1259 OID 36335)
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    role_id integer NOT NULL,
    role_name character varying(50) NOT NULL
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- TOC entry 225 (class 1259 OID 36334)
-- Name: roles_role_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_role_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roles_role_id_seq OWNER TO postgres;

--
-- TOC entry 5089 (class 0 OID 0)
-- Dependencies: 225
-- Name: roles_role_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_role_id_seq OWNED BY public.roles.role_id;


--
-- TOC entry 228 (class 1259 OID 36346)
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.users (
    user_id integer NOT NULL,
    login character varying(50) NOT NULL,
    password character varying(100) NOT NULL,
    full_name character varying(150) NOT NULL,
    role_id integer NOT NULL,
    is_active boolean DEFAULT true NOT NULL,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    email character varying(150) NOT NULL
);


ALTER TABLE public.users OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 36345)
-- Name: users_user_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_user_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.users_user_id_seq OWNER TO postgres;

--
-- TOC entry 5090 (class 0 OID 0)
-- Dependencies: 227
-- Name: users_user_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_user_id_seq OWNED BY public.users.user_id;


--
-- TOC entry 4881 (class 2604 OID 36284)
-- Name: clients client_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clients ALTER COLUMN client_id SET DEFAULT nextval('public.clients_client_id_seq'::regclass);


--
-- TOC entry 4884 (class 2604 OID 36311)
-- Name: orders order_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN order_id SET DEFAULT nextval('public.orders_order_id_seq'::regclass);


--
-- TOC entry 4891 (class 2604 OID 36372)
-- Name: password_reset_requests request_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.password_reset_requests ALTER COLUMN request_id SET DEFAULT nextval('public.password_reset_requests_request_id_seq'::regclass);


--
-- TOC entry 4882 (class 2604 OID 36297)
-- Name: products product_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products ALTER COLUMN product_id SET DEFAULT nextval('public.products_product_id_seq'::regclass);


--
-- TOC entry 4887 (class 2604 OID 36338)
-- Name: roles role_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN role_id SET DEFAULT nextval('public.roles_role_id_seq'::regclass);


--
-- TOC entry 4888 (class 2604 OID 36349)
-- Name: users user_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN user_id SET DEFAULT nextval('public.users_user_id_seq'::regclass);


--
-- TOC entry 5069 (class 0 OID 36281)
-- Dependencies: 220
-- Data for Name: clients; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.clients (client_id, full_name, phone, email, address) FROM stdin;
1	Иванов Иван Иванович	+79990000001	ivanov@mail.ru	г. Москва
2	Петров Петр Сергеевич	+79990000002	petrov@mail.ru	г. Казань
3	Сидорова Анна Андреевна	+79990000003	sidorova@mail.ru	г. Самара
4	Смирнов Алексей Олегович	+79990000004	smirnov@mail.ru	г. Санкт-Петербург
5	Кузнецова Мария Викторовна	+79990000005	kuznetsova@mail.ru	г. Пермь
\.


--
-- TOC entry 5073 (class 0 OID 36308)
-- Dependencies: 224
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (order_id, client_id, product_id, quantity, order_date, status) FROM stdin;
1	1	1	1	2026-09-09 14:28:54.692673	Новый
2	2	2	2	2026-09-09 14:28:54.692673	В обработке
3	3	4	1	2026-09-09 14:28:54.692673	Выполнен
4	4	3	3	2026-09-09 14:28:54.692673	Новый
5	5	5	1	2026-09-09 14:28:54.692673	В обработке
6	1	6	1	2026-09-09 14:28:54.692673	Выполнен
7	2	7	2	2026-09-09 14:28:54.692673	Новый
\.


--
-- TOC entry 5079 (class 0 OID 36369)
-- Dependencies: 230
-- Data for Name: password_reset_requests; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.password_reset_requests (request_id, user_id, request_date, status, completed_date) FROM stdin;
1	2	2026-09-18 16:56:04.929847	Выполнена	2026-09-18 16:56:19.404468
2	2	2026-09-19 15:40:14.246352	Выполнена	2026-09-19 15:40:34.422015
3	2	2026-09-19 15:47:28.266611	Выполнена	2026-09-19 15:47:42.48866
\.


--
-- TOC entry 5071 (class 0 OID 36294)
-- Dependencies: 222
-- Data for Name: products; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.products (product_id, product_name, price, quantity_in_stock) FROM stdin;
1	Ноутбук	65000.00	10
2	Монитор	18000.00	15
3	Клавиатура	3500.00	25
4	Мышь	2200.00	30
5	Наушники	4500.00	20
6	Принтер	15000.00	8
7	Веб-камера	5200.00	12
\.


--
-- TOC entry 5075 (class 0 OID 36335)
-- Dependencies: 226
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (role_id, role_name) FROM stdin;
1	Администратор
2	Менеджер
\.


--
-- TOC entry 5077 (class 0 OID 36346)
-- Dependencies: 228
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (user_id, login, password, full_name, role_id, is_active, created_at, email) FROM stdin;
1	admin	1234	Администратор системы	1	t	2026-09-18 16:35:22.708049	admin@ShopMagazik.ru
2	manager	1111	Иванов Иван Иванович	2	t	2026-09-18 16:35:22.708049	manager@ShopMagazik.ru
4	sergeev.ss	1111	Сергеев Сергей Сергеевич	2	t	2026-09-19 15:56:01.755457	sergeev.ss@ShopMagazik.ru
5	sergeev.ss2	1111	Сергеев Сергей Сергеевич	2	t	2026-09-19 16:17:52.8339	sergeev.ss2@ShopMagazik.ru
\.


--
-- TOC entry 5091 (class 0 OID 0)
-- Dependencies: 219
-- Name: clients_client_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.clients_client_id_seq', 5, true);


--
-- TOC entry 5092 (class 0 OID 0)
-- Dependencies: 223
-- Name: orders_order_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_order_id_seq', 7, true);


--
-- TOC entry 5093 (class 0 OID 0)
-- Dependencies: 229
-- Name: password_reset_requests_request_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.password_reset_requests_request_id_seq', 3, true);


--
-- TOC entry 5094 (class 0 OID 0)
-- Dependencies: 221
-- Name: products_product_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.products_product_id_seq', 7, true);


--
-- TOC entry 5095 (class 0 OID 0)
-- Dependencies: 225
-- Name: roles_role_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_role_id_seq', 2, true);


--
-- TOC entry 5096 (class 0 OID 0)
-- Dependencies: 227
-- Name: users_user_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_user_id_seq', 5, true);


--
-- TOC entry 4898 (class 2606 OID 36292)
-- Name: clients clients_email_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clients
    ADD CONSTRAINT clients_email_key UNIQUE (email);


--
-- TOC entry 4900 (class 2606 OID 36290)
-- Name: clients clients_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.clients
    ADD CONSTRAINT clients_pkey PRIMARY KEY (client_id);


--
-- TOC entry 4904 (class 2606 OID 36322)
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (order_id);


--
-- TOC entry 4916 (class 2606 OID 36380)
-- Name: password_reset_requests password_reset_requests_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.password_reset_requests
    ADD CONSTRAINT password_reset_requests_pkey PRIMARY KEY (request_id);


--
-- TOC entry 4902 (class 2606 OID 36306)
-- Name: products products_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.products
    ADD CONSTRAINT products_pkey PRIMARY KEY (product_id);


--
-- TOC entry 4906 (class 2606 OID 36342)
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (role_id);


--
-- TOC entry 4908 (class 2606 OID 36344)
-- Name: roles roles_role_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_role_name_key UNIQUE (role_name);


--
-- TOC entry 4910 (class 2606 OID 36388)
-- Name: users users_email_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_email_unique UNIQUE (email);


--
-- TOC entry 4912 (class 2606 OID 36360)
-- Name: users users_login_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_login_key UNIQUE (login);


--
-- TOC entry 4914 (class 2606 OID 36358)
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (user_id);


--
-- TOC entry 4920 (class 2606 OID 36381)
-- Name: password_reset_requests fk_reset_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.password_reset_requests
    ADD CONSTRAINT fk_reset_user FOREIGN KEY (user_id) REFERENCES public.users(user_id) ON DELETE CASCADE;


--
-- TOC entry 4919 (class 2606 OID 36361)
-- Name: users fk_user_role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT fk_user_role FOREIGN KEY (role_id) REFERENCES public.roles(role_id) ON DELETE RESTRICT;


--
-- TOC entry 4917 (class 2606 OID 36323)
-- Name: orders orders_client_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_client_id_fkey FOREIGN KEY (client_id) REFERENCES public.clients(client_id);


--
-- TOC entry 4918 (class 2606 OID 36328)
-- Name: orders orders_product_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_product_id_fkey FOREIGN KEY (product_id) REFERENCES public.products(product_id);


-- Completed on 2026-11-02 22:45:01

--
-- PostgreSQL database dump complete
--

\unrestrict rlY0jTCqXdJHWwe8KwevndYy0gh4OLrT60ZgQeh3gk37fFJxWdzhSE4mL8SaCrY


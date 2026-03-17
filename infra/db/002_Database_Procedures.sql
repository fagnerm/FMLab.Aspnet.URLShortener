CREATE OR REPLACE FUNCTION fn_daily_clicks(hash_id TEXT, since timestamp with time zone)
RETURNS TABLE(year INT, month INT, day INT, count INT)
AS $$
	select
		date_part('year', uc.clicked_at )
		,date_part('month', uc.clicked_at )
		,date_part('day', uc.clicked_at )
		,count(uc.*) as count
	from
		short_urls su 
		left join url_clicks uc on uc.hash = su.hash
	where
		su.hash = hash_id
		and uc.clicked_at >= since
	group by 
		date_part('year', uc.clicked_at )
		,date_part('month', uc.clicked_at )
		,date_part('day', uc.clicked_at )
	order by count desc;
$$ LANGUAGE sql;

/*******************************************************************/

CREATE OR REPLACE FUNCTION fn_top_referers(hash_id TEXT, top INTEGER)
RETURNS TABLE(referer VARCHAR, count INT)
AS $$
	select
		uc.referer
		,count(*) as count
	from
		short_urls su 
		left join url_clicks uc on uc.hash = su.hash
	where
		su.hash = hash_id
	group by 
		uc.referer
	order by count desc
	limit top;
$$ LANGUAGE sql;
CREATE TABLE [dbo].[T_WebPage]
(
	[ID] BIGINT NOT NULL PRIMARY KEY, 
    [url] NVARCHAR(MAX) NOT NULL, 
    [p_url] NVARCHAR(MAX) NULL, 
    [key_words] NVARCHAR(MAX) NULL, 
    [content_abstract] NVARCHAR(500) NULL, 
    [create_time] DATETIME NULL, 
    [update_time] DATETIME NULL
)
